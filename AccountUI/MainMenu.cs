using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading; // Cần cái này cho WPF Dispatcher
using ChessClient; // Đảm bảo namespace này đúng với project của bạn

namespace AccountUI
{
    public partial class MainMenu : Form
    {
        private bool _isListening = false;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            // Có thể thêm code khởi tạo nếu cần
        }

        // =================================================================================
        // KHU VỰC XỬ LÝ SỰ KIỆN BUTTON (Gửi yêu cầu lên Server)
        // =================================================================================

        private async void button1_Click(object sender, EventArgs e)
        {
            // Nút Ghép trận ngẫu nhiên
            await SendReq("FIND_GAME", "Đang tìm đối thủ...", button1);
        }

        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            // Nút Tạo phòng
            await SendReq("CREATE_ROOM", "Đang tạo phòng...", btnCreateRoom);
        }

        private async void btnJoinRoom_Click(object sender, EventArgs e)
        {
            // Nút Vào phòng
            if (string.IsNullOrEmpty(txtRoomId.Text))
            {
                MessageBox.Show("Vui lòng nhập ID phòng!");
                return;
            }
            await SendReq($"JOIN_ROOM|{txtRoomId.Text}", "Đang vào phòng...", btnJoinRoom);
        }

        /// <summary>
        /// Hàm chung để gửi yêu cầu lên server và bắt đầu lắng nghe phản hồi
        /// </summary>
        private async Task SendReq(string cmd, string waitText, Button btnTrigger)
        {
            if (_isListening) return; // Tránh bấm nhiều lần

            if (!ClientManager.Instance.IsConnected)
            {
                MessageBox.Show("Mất kết nối với máy chủ!");
                return;
            }

            try
            {
                _isListening = true;

                // Khóa giao diện
                btnTrigger.Text = waitText;
                DisableButtons();

                // Gửi lệnh
                await ClientManager.Instance.SendAsync(cmd);

                // Bắt đầu vòng lặp lắng nghe
                await ListenLoop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi gửi yêu cầu: {ex.Message}");
                ResetUI();
            }
        }

        // =================================================================================
        // KHU VỰC LẮNG NGHE PHẢN HỒI TỪ SERVER
        // =================================================================================

        private async Task ListenLoop()
        {
            await Task.Run(() =>
            {
                while (_isListening)
                {
                    try
                    {
                        // Đợi tin nhắn từ Server
                        string msg = ClientManager.Instance.WaitForMessage();

                        // Nếu mất kết nối hoặc msg null
                        if (string.IsNullOrEmpty(msg))
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show("Mất kết nối server!");
                                ResetUI();
                            }));
                            break;
                        }

                        // --- XỬ LÝ CÁC LOẠI TIN NHẮN ---

                        // 1. Game bắt đầu -> Mở bàn cờ
                        if (msg.StartsWith("GAME_START"))
                        {
                            _isListening = false;
                            LaunchGame(msg); // Gọi hàm mở WPF
                            break;
                        }
                        // 2. Tạo phòng thành công -> Hiển thị ID
                        else if (msg.StartsWith("ROOM_CREATED"))
                        {
                            // msg dạng: ROOM_CREATED|12345
                            string id = msg.Split('|')[1];
                            this.Invoke((MethodInvoker)(() =>
                            {
                                txtRoomId.Text = id;
                                labelRoom.Text = "Mã phòng: " + id;
                                btnCreateRoom.Text = "Đang chờ người vào...";
                            }));
                        }
                        // 3. Có lỗi (Sai ID phòng, Phòng đầy...)
                        else if (msg.StartsWith("ERROR") || msg.StartsWith("ROOM_ERROR"))
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show(msg);
                                ResetUI();
                            }));
                            break;
                        }
                        // 4. Trạng thái chờ
                        else if (msg.StartsWith("WAITING"))
                        {
                            this.Invoke((MethodInvoker)(() => button1.Text = "Đang đợi..."));
                        }
                    }
                    catch
                    {
                        _isListening = false;
                        break;
                    }
                }
            });
        }

        // =================================================================================
        // KHU VỰC MỞ BÀN CỜ WPF (Đã sửa lỗi Threading)
        // =================================================================================

        private void LaunchGame(string gameStartMessage)
        {
            Thread wpfThread = new Thread(() =>
            {
                try
                {
                    ChessUI.MainWindow gameWindow = new ChessUI.MainWindow(gameStartMessage);

                    // Khi bàn cờ load xong thì ẩn MainMenu
                    gameWindow.Loaded += (s, e) =>
                    {
                        this.Invoke((MethodInvoker)delegate { this.Hide(); });
                    };

                    // Khi bàn cờ đóng lại thì hiện MainMenu
                    gameWindow.Closed += (s, e) =>
                    {
                        try
                        {
                            // Tắt Dispatcher WPF
                            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Background);

                            if (!this.IsDisposed && this.IsHandleCreated)
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.Show();
                                    this.WindowState = FormWindowState.Normal;
                                    this.BringToFront();
                                    ResetUI();
                                });
                            }
                        }
                        catch { }
                    };

                    gameWindow.Show();
                    System.Windows.Threading.Dispatcher.Run(); // Bắt buộc phải có dòng này
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        MessageBox.Show("Lỗi mở bàn cờ: " + ex.Message);
                        ResetUI();
                        this.Show();
                    }));
                }
            });

            wpfThread.SetApartmentState(ApartmentState.STA); // Bắt buộc cho WPF
            wpfThread.IsBackground = true;
            wpfThread.Start();
        }

        // =================================================================================
        // KHU VỰC HÀM TIỆN ÍCH & CÁC NÚT KHÁC
        // =================================================================================

        private void DisableButtons()
        {
            button1.Enabled = false;
            btnCreateRoom.Enabled = false;
            btnJoinRoom.Enabled = false;
        }

        private void ResetUI()
        {
            _isListening = false;

            button1.Text = "Ghép Ngẫu Nhiên";
            btnCreateRoom.Text = "Tạo Phòng Mới";
            btnJoinRoom.Text = "Vào Phòng";

            button1.Enabled = true;
            btnCreateRoom.Enabled = true;
            btnJoinRoom.Enabled = true;

            labelRoom.Text = ""; // Xóa mã phòng cũ nếu có
        }

        private void btnFriend_Click(object sender, EventArgs e)
        {
            Friend frm = new Friend();
            frm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Đăng xuất
            ClientManager.Disconnect();
            Application.Exit();
        }

        // Các nút cũ hoặc chưa dùng tới
        private void button3_Click(object sender, EventArgs e) { }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            string conn = "Server=(localdb)\\MSSQLLocalDB;Database=NetChessDB;Trusted_Connection=True;";

            if (string.IsNullOrEmpty(ClientManager.Username))
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
                return;
            }

            var profileWin = new ChessUI.ProfileWindow(ClientManager.Username, conn);
            profileWin.ShowDialog();
        }
    }
}