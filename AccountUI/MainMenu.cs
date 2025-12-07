#nullable disable
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using ChessClient;

namespace AccountUI
{
    public partial class MainMenu : Form
    {
        private bool _isListening = false;
        private MatchFoundForm _currentMatchForm;

        public MainMenu()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            CheckForIllegalCrossThreadCalls = false;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            ResetUI();
        }

        // ==========================================================
        //  NÚT PROFILE  (ĐÃ FIX: KHÔNG DÙNG THREAD RIÊNG NỮA)
        // ==========================================================
        private void btnProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ClientManager.Username))
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
                return;
            }

            try
            {
                // WinForms chạy trên STA nên có thể mở WPF trực tiếp
                var win = new ChessUI.ProfileWindow(ClientManager.Username);
                win.ShowDialog(); // Mở dạng modal cho đơn giản, an toàn
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở Profile: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================================
        // ❗ DƯỚI ĐÂY LÀ NGUYÊN CODE GỐC CỦA BẠN — KHÔNG ĐỤNG TỚI
        // =====================================================================

        private async void button1_Click(object sender, EventArgs e)
        {
            await SendRequest("FIND_GAME", "Đang tìm đối thủ...", button1);
        }

        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            await SendRequest("CREATE_ROOM", "Đang tạo phòng...", btnCreateRoom);
        }

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

        private async Task SendRequest(string command, string waitText, Button clickedButton)
        {
            if (_isListening) return;

            if (!ClientManager.Instance.IsConnected)
            {
                MessageBox.Show("Mất kết nối tới máy chủ! Vui lòng đăng nhập lại.");
                return;
            }

            try
            {
                _isListening = true;

                button1.Enabled = false;
                btnCreateRoom.Enabled = false;
                btnJoinRoom.Enabled = false;

                clickedButton.Text = waitText;

                await ClientManager.Instance.SendAsync(command);
                await ListenForMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                ResetUI();
            }
        }

        private async Task ListenForMessages()
        {
            await Task.Run(() =>
            {
                while (_isListening)
                {
                    try
                    {
                        string message = ClientManager.Instance.WaitForMessage();

                        if (string.IsNullOrEmpty(message))
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show("Mất kết nối tới máy chủ!");
                                CloseMatchFormIfAny();
                                ResetUI();
                            }));
                            _isListening = false;
                            return;
                        }

                        if (message.StartsWith("GAME_START"))
                        {
                            _isListening = false;

                            this.Invoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();
                                this.Hide();
                                LaunchWpfGameWindow(message);
                            }));

                            return;
                        }

                        else if (message.StartsWith("MATCH_FOUND"))
                        {
                            string[] parts = message.Split('|');
                            string roomId = parts.Length > 1 ? parts[1] : "";

                            this.BeginInvoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();

                                var form = new MatchFoundForm();
                                form.StartPosition = FormStartPosition.Manual;
                                form.Location = new Point(
                                    this.Location.X + (this.Width - form.Width) / 2,
                                    this.Location.Y + (this.Height - form.Height) / 2
                                );

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

                        else if (message.StartsWith("MATCH_CANCELLED"))
                        {
                            _isListening = false;

                            this.Invoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();
                                MessageBox.Show("Trận đấu bị hủy.");
                                ResetUI();
                            }));

                            return;
                        }

                        else if (message.StartsWith("ROOM_CREATED"))
                        {
                            string[] parts = message.Split('|');
                            string id = parts.Length > 1 ? parts[1] : "";

                            this.Invoke((MethodInvoker)(() =>
                            {
                                txtRoomId.Text = id;
                                labelRoom.Text = "Mã phòng: " + id;
                                btnCreateRoom.Text = "Đang chờ người chơi...";
                            }));
                        }

                        else if (message.StartsWith("ERROR"))
                        {
                            _isListening = false;

                            this.Invoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();
                                MessageBox.Show(message);
                                ResetUI();
                            }));

                            return;
                        }

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

        private void LaunchWpfGameWindow(string gameStartMessage)
        {
            Thread wpfThread = new Thread(() =>
            {
                try
                {
                    var gameWindow = new ChessUI.MainWindow(gameStartMessage);

                    gameWindow.Closed += (s, e) =>
                    {
                        Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                        if (!this.IsDisposed)
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                this.Show();
                                this.WindowState = FormWindowState.Normal;
                                this.BringToFront();
                                ResetUI();
                            }));
                        }
                    };

                    gameWindow.Show();
                    Dispatcher.Run();
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

            wpfThread.SetApartmentState(ApartmentState.STA);
            wpfThread.Start();
        }

        private void ResetUI()
        {
            _isListening = false;

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
    }
}
