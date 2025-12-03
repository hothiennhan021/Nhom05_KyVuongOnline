#nullable disable
using System;
using System.Drawing; // For Point, Size
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading; // For WPF dispatcher
using ChessClient;              // ClientManager

namespace AccountUI
{
    public partial class MainMenu : Form
    {
        private bool _isListening = false;
        private MatchFoundForm _currentMatchForm;

        public MainMenu()
        {
            InitializeComponent();
            // Đảm bảo Form luôn ở giữa
            this.StartPosition = FormStartPosition.CenterScreen;
            // Cho phép cập nhật UI từ luồng khác để tránh lỗi Cross-thread
            CheckForIllegalCrossThreadCalls = false;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            ResetUI();
        }

        // ============================================================
        // 1. GỬI LỆNH LÊN SERVER
        // ============================================================

        // Tìm trận ngẫu nhiên
        private async void button1_Click(object sender, EventArgs e)
        {
            await SendRequest("FIND_GAME", "Đang tìm đối thủ...", button1);
        }

        // Tạo phòng
        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            await SendRequest("CREATE_ROOM", "Đang tạo phòng...", btnCreateRoom);
        }

        // Vào phòng
        private async void btnJoinRoom_Click(object sender, EventArgs e)
        {
            string roomId = txtRoomId.Text.Trim();
            if (string.IsNullOrEmpty(roomId))
            {
                MessageBox.Show("Vui lòng nhập ID phòng!");
                return;
            }
            await SendRequest($"JOIN_ROOM|{roomId}", "Đang vào phòng...", btnJoinRoom);
        }

        // Nút Profile
        private void btnProfile_Click(object sender, EventArgs e)
        {
            string conn = "Server=(localdb)\\MSSQLLocalDB;Database=ChessDB;Trusted_Connection=True;";

            if (string.IsNullOrEmpty(ClientManager.Username))
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
                return;
            }

            var profileWin = new ChessUI.ProfileWindow(ClientManager.Username, conn);
            profileWin.ShowDialog();
        }

        /// <summary>
        /// Gửi lệnh và bắt đầu lắng nghe phản hồi
        /// </summary>
        private async Task SendRequest(string command, string waitText, Button clickedButton)
        {
            // Nếu đang lắng nghe rồi thì thôi, tránh tạo nhiều luồng song song
            if (_isListening) return;

            if (!ClientManager.Instance.IsConnected)
            {
                MessageBox.Show("Mất kết nối tới máy chủ! Vui lòng đăng nhập lại.");
                return;
            }

            try
            {
                _isListening = true;

                // Disable các nút để tránh spam lệnh
                button1.Enabled = false;
                btnCreateRoom.Enabled = false;
                btnJoinRoom.Enabled = false;

                clickedButton.Text = waitText;

                // Gửi lệnh đi
                await ClientManager.Instance.SendAsync(command);

                // Bắt đầu vòng lặp lắng nghe
                await ListenForMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                ResetUI();
            }
        }

        // ============================================================
        // 2. VÒNG LẶP LẮNG NGHE (ĐÃ FIX LỖI TRANH CHẤP)
        // ============================================================

        private async Task ListenForMessages()
        {
            await Task.Run(() =>
            {
                while (_isListening)
                {
                    try
                    {
                        // Đọc tin nhắn từ Server (Blocking call)
                        string message = ClientManager.Instance.WaitForMessage();

                        // Nếu mất kết nối
                        if (string.IsNullOrEmpty(message))
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show("Mất kết nối tới máy chủ!");
                                CloseMatchFormIfAny();
                                ResetUI();
                            }));
                            _isListening = false;
                            return; // Thoát Task
                        }

                        // --------------------------------------------------------
                        // 1) GAME_START -> Vào Game (QUAN TRỌNG NHẤT)
                        // --------------------------------------------------------
                        if (message.StartsWith("GAME_START"))
                        {
                            // [FIX]: Ngắt cờ lắng nghe ngay lập tức để luồng này không đọc thêm gì nữa
                            _isListening = false;

                            this.Invoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();
                                this.Hide();
                                LaunchWpfGameWindow(message);
                            }));

                            // [FIX]: Dùng return để kill Task ngay lập tức, đảm bảo Socket rảnh cho Game
                            return;
                        }
                        // 2) MATCH_FOUND -> Hiện Popup xác nhận
                        else if (message.StartsWith("MATCH_FOUND"))
                        {
                            string[] parts = message.Split('|');
                            string roomId = parts.Length > 1 ? parts[1] : "";

                            this.BeginInvoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();

                                var form = new MatchFoundForm();
                                // Căn giữa Popup
                                form.StartPosition = FormStartPosition.Manual;
                                form.Location = new Point(
                                    this.Location.X + (this.Width - form.Width) / 2,
                                    this.Location.Y + (this.Height - form.Height) / 2
                                );

                                // Xử lý sự kiện Accept/Decline
                                form.Accepted += () =>
                                {
                                    _ = ClientManager.Instance.SendAsync($"MATCH_RESPONSE|{roomId}|ACCEPT");
                                };

                                form.Declined += () =>
                                {
                                    _ = ClientManager.Instance.SendAsync($"MATCH_RESPONSE|{roomId}|DECLINE");
                                    _isListening = false;
                                    ResetUI();
                                    _currentMatchForm = null;
                                };

                                form.FormClosed += (s, ev) =>
                                {
                                    if (_currentMatchForm == form) _currentMatchForm = null;
                                };

                                _currentMatchForm = form;
                                form.Show(this);
                            }));
                        }
                        // 3) MATCH_CANCELLED
                        else if (message.StartsWith("MATCH_CANCELLED"))
                        {
                            _isListening = false; // Dừng tìm kiếm

                            string reason = "Trận đấu đã bị hủy.";
                            string[] parts = message.Split('|');
                            if (parts.Length > 1) reason = parts[1];

                            this.Invoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();
                                MessageBox.Show(reason, "Ghép trận thất bại");
                                ResetUI();
                            }));
                            return; // Thoát Task
                        }
                        // 4) ROOM_CREATED
                        else if (message.StartsWith("ROOM_CREATED"))
                        {
                            string[] parts = message.Split('|');
                            string id = parts.Length > 1 ? parts[1] : "";

                            this.Invoke((MethodInvoker)(() =>
                            {
                                txtRoomId.Text = id;
                                labelRoom.Text = "Mã phòng: " + id;
                                btnCreateRoom.Text = "Đang chờ người vào...";
                            }));
                        }
                        // 5) ERROR
                        else if (message.StartsWith("ERROR") || message.StartsWith("ROOM_ERROR"))
                        {
                            _isListening = false;
                            this.Invoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();
                                MessageBox.Show(message, "Lỗi");
                                ResetUI();
                            }));
                            return; // Thoát Task
                        }
                        // 6) WAITING
                        else if (message.StartsWith("WAITING"))
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                button1.Text = "Đang đợi đối thủ...";
                            }));
                        }
                    }
                    catch
                    {
                        // Nếu có lỗi socket hoặc luồng bị hủy
                        _isListening = false;
                        this.Invoke((MethodInvoker)(() =>
                        {
                            CloseMatchFormIfAny();
                            ResetUI();
                        }));
                        return;
                    }
                }
            });
        }

        private void CloseMatchFormIfAny()
        {
            try
            {
                if (_currentMatchForm != null && !_currentMatchForm.IsDisposed)
                {
                    _currentMatchForm.Close();
                    _currentMatchForm = null;
                }
            }
            catch { }
        }

        // ============================================================
        // 3. LAUNCH WPF WINDOW (GAME)
        // ============================================================

        private void LaunchWpfGameWindow(string gameStartMessage)
        {
            // Tạo một Thread mới dành riêng cho WPF Window
            Thread wpfThread = new Thread(() =>
            {
                try
                {
                    // 
                    var gameWindow = new ChessUI.MainWindow(gameStartMessage);

                    // Khi tắt Game, hiện lại Menu
                    gameWindow.Closed += (s, e) =>
                    {
                        try
                        {
                            // Tắt Dispatcher của WPF
                            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                            // Quay về luồng UI của MainMenu để hiện form
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
                    Dispatcher.Run(); // Bắt đầu vòng lặp message của WPF
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        MessageBox.Show("Lỗi khởi tạo bàn cờ: " + ex.Message);
                        ResetUI();
                        this.Show();
                    }));
                }
            });

            wpfThread.SetApartmentState(ApartmentState.STA); // Bắt buộc cho WPF
            wpfThread.IsBackground = false;
            wpfThread.Start();
        }

        // ============================================================
        // 4. UI UTILS
        // ============================================================

        private void ResetUI()
        {
            _isListening = false; // Đảm bảo trạng thái clean

            if (button1.InvokeRequired)
            {
                this.Invoke((MethodInvoker)ResetUI);
                return;
            }

            button1.Text = "Ghép Trận Ngẫu Nhiên";
            btnCreateRoom.Text = "Tạo Phòng";
            btnJoinRoom.Text = "Vào Phòng";

            button1.Enabled = true;
            btnCreateRoom.Enabled = true;
            btnJoinRoom.Enabled = true;

            labelRoom.Text = "";
        }

        // ============================================================
        // 5. OTHER BUTTONS
        // ============================================================

        private void btnFriend_Click(object sender, EventArgs e)
        {
            Friend frm = new Friend();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try { ClientManager.Disconnect(); } catch { }
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e) { }
    }
}