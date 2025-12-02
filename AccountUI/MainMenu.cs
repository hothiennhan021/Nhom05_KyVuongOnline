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
            // Ensure MainMenu starts in the center
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            ResetUI();
        }

        // ============================================================
        // 1. SEND COMMANDS TO SERVER
        // ============================================================

        // Find Random Match
        private async void button1_Click(object sender, EventArgs e)
        {
            await SendRequest("FIND_GAME", "Đang tìm đối thủ...", button1);
        }

        // Create Room
        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            await SendRequest("CREATE_ROOM", "Đang tạo phòng...", btnCreateRoom);
        }

        // Join Room
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

        // [FIXED] Missing event handler for Profile button
        private void btnProfile_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Hồ Sơ đang phát triển!");
            // You can implement the actual profile form opening here later
        }

        /// <summary>
        /// Sends a command and starts listening for a response.
        /// </summary>
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

                // Disable buttons
                button1.Enabled = false;
                btnCreateRoom.Enabled = false;
                btnJoinRoom.Enabled = false;

                clickedButton.Text = waitText;

                // Send command
                await ClientManager.Instance.SendAsync(command);

                // Start listening
                await ListenForMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                ResetUI();
            }
        }

        // ============================================================
        // 2. LISTEN FOR SERVER MESSAGES
        // ============================================================

        private async Task ListenForMessages()
        {
            await Task.Run(() =>
            {
                while (_isListening)
                {
                    try
                    {
                        string message = ClientManager.Instance.WaitForMessage();

                        // Disconnected
                        if (string.IsNullOrEmpty(message))
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show("Mất kết nối tới máy chủ!");
                                CloseMatchFormIfAny();
                                ResetUI();
                            }));
                            _isListening = false;
                            break;
                        }

                        // 1) GAME_START -> Open WPF Board
                        if (message.StartsWith("GAME_START"))
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                _isListening = false;
                                CloseMatchFormIfAny();
                                this.Hide();
                                LaunchWpfGameWindow(message);
                            }));
                            break;
                        }
                        // 2) MATCH_FOUND -> Show Popup
                        else if (message.StartsWith("MATCH_FOUND"))
                        {
                            // Format: MATCH_FOUND|roomId
                            string[] parts = message.Split('|');
                            string roomId = parts.Length > 1 ? parts[1] : "";

                            this.BeginInvoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny(); // Close old one if exists

                                var form = new MatchFoundForm();

                                // Center popup on MainMenu
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

                                form.FormClosed += (s, e) =>
                                {
                                    if (_currentMatchForm == form) _currentMatchForm = null;
                                };

                                _currentMatchForm = form;
                                form.Show(this); // Show as modeless dialog owned by MainMenu
                            }));
                        }
                        // 3) MATCH_CANCELLED
                        else if (message.StartsWith("MATCH_CANCELLED"))
                        {
                            string reason = "Trận đấu đã bị hủy.";
                            string[] parts = message.Split('|');
                            if (parts.Length > 1) reason = parts[1];

                            this.Invoke((MethodInvoker)(() =>
                            {
                                _isListening = false;
                                CloseMatchFormIfAny();
                                MessageBox.Show(reason, "Ghép trận thất bại");
                                ResetUI();
                            }));
                            break;
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
                            this.Invoke((MethodInvoker)(() =>
                            {
                                CloseMatchFormIfAny();
                                MessageBox.Show(message, "Lỗi");
                                ResetUI();
                            }));
                            _isListening = false;
                            break;
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
                        this.Invoke((MethodInvoker)(() =>
                        {
                            CloseMatchFormIfAny();
                            ResetUI();
                        }));
                        _isListening = false;
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
        // 3. LAUNCH WPF WINDOW
        // ============================================================

        private void LaunchWpfGameWindow(string gameStartMessage)
        {
            Thread wpfThread = new Thread(() =>
            {
                try
                {
                    var gameWindow = new ChessUI.MainWindow(gameStartMessage);

                    gameWindow.Closed += (s, e) =>
                    {
                        try
                        {
                            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

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
            wpfThread.IsBackground = false;
            wpfThread.Start();
        }

        // ============================================================
        // 4. UI UTILS
        // ============================================================

        private void ResetUI()
        {
            _isListening = false;

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