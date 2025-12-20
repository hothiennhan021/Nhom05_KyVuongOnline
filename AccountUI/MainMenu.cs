#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using ChessClient;
using ChessUI;

namespace AccountUI
{
    public partial class MainMenu : Form
    {
        private bool _isListening = false;
        private volatile bool _isLoggingOut = false;
        private bool _allowCloseProgrammatically = false;
        private MatchFoundForm _currentMatchForm;

        private const string PLACEHOLDER_JOIN = "Nhập ID Phòng";

        private enum UIMode
        {
            Idle,
            Matching,
            Creating,
            Joining
        }

        private UIMode _mode = UIMode.Idle;

        public MainMenu()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            CheckForIllegalCrossThreadCalls = false;

            this.Load += MainMenu_Load;
            this.Resize += (s, e) => CenterCard();
            this.MaximizeBox = false;
            this.FormClosing += MainMenu_FormClosing;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            CenterCard();
            SetupLogo();
            SetupPlaceholders();
            RoundEverything();

            pnlCreatedId.Visible = false;
            pnlJoinId.Visible = false;
            btnCancelMatch.Visible = false;

            // Đưa nút lên lớp trên cùng
            btnProfile.BringToFront();
            btnFriend.BringToFront();
            button1.BringToFront();
            btnCreateRoom.BringToFront();
            btnJoinRoom.BringToFront();
            button4.BringToFront();

            ResetUI();
        }

        private void CenterCard()
        {
            if (panelCard == null) return;
            panelCard.Left = (this.ClientSize.Width - panelCard.Width) / 2;
            panelCard.Top = (this.ClientSize.Height - panelCard.Height) / 2;
        }

        private void SetupLogo()
        {
            try
            {
                picKnight.Image = Properties.Resources.icon_knight;
                picKnight.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch { }

            int iconSize = 60;
            panelKnightBg.Size = new Size(iconSize, iconSize);
            picKnight.Dock = DockStyle.Fill;

            lblChess.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
            lblOnline.Font = new Font("Segoe UI", 9f, FontStyle.Regular);

            int gapHorizontal = 12;
            int gapVertical = 2;
            int textWidth = Math.Max(lblChess.Width, lblOnline.Width);
            int textHeight = lblChess.Height + gapVertical + lblOnline.Height;
            int blockWidth = iconSize + gapHorizontal + textWidth;
            int blockHeight = Math.Max(iconSize, textHeight);

            panelIcon.Size = new Size(blockWidth, blockHeight);
            panelIcon.Left = (panelCard.Width - panelIcon.Width) / 2;
            panelKnightBg.Location = new Point(0, (panelIcon.Height - panelKnightBg.Height) / 2);

            int textStartX = panelKnightBg.Right + gapHorizontal;
            int textStartY = (panelIcon.Height - textHeight) / 2;
            lblChess.Location = new Point(textStartX, textStartY);
            lblOnline.Location = new Point(textStartX + (lblChess.Width - lblOnline.Width) / 2, lblChess.Bottom + gapVertical);

            lblTitle.Left = (panelCard.Width - lblTitle.Width) / 2;
        }

        private void SetupPlaceholders()
        {
            // === Ô NHẬP ID (VÀO PHÒNG) ===
            txtRoomId.TextAlign = HorizontalAlignment.Center;
            txtRoomId.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            txtRoomId.Text = PLACEHOLDER_JOIN;
            txtRoomId.ForeColor = Color.Gray;

            txtRoomId.GotFocus += (s, e) =>
            {
                if (txtRoomId.Text == PLACEHOLDER_JOIN)
                {
                    txtRoomId.Text = "";
                    txtRoomId.ForeColor = Color.White;
                }
            };

            txtRoomId.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtRoomId.Text))
                {
                    txtRoomId.Text = PLACEHOLDER_JOIN;
                    txtRoomId.ForeColor = Color.Gray;
                }
            };

            // === Ô HIỆN ID (TẠO PHÒNG) ===
            txtCreatedRoomId.ReadOnly = true;
            txtCreatedRoomId.TextAlign = HorizontalAlignment.Center;
            txtCreatedRoomId.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            txtCreatedRoomId.BackColor = Color.FromArgb(35, 45, 65);
            txtCreatedRoomId.Text = "";
            // ĐỔI MÀU Ở ĐÂY: Xanh Ngọc (Cyan) sáng, dễ nhìn
            txtCreatedRoomId.ForeColor = Color.Cyan;

            labelRoom.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            labelRoom.ForeColor = Color.White;
        }

        private void RoundEverything()
        {
            RoundControl(btnProfile, 22);
            RoundControl(btnFriend, 22);
            RoundControl(button1, 22);
            RoundControl(btnCancelMatch, 22);
            RoundControl(btnCreateRoom, 22);
            RoundControl(btnJoinRoom, 22);
            RoundControl(button4, 18);
            RoundControl(pnlCreatedId, 16);
            RoundControl(pnlJoinId, 16);
        }

        private void RoundControl(Control control, int radius)
        {
            if (control == null) return;
            if (control.Width <= 0 || control.Height <= 0) return;
            Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
            int d = radius * 2;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseAllFigures();
                control.Region = new Region(path);
            }
        }

        private void ShowCancelBelow(Control target, string text)
        {
            if (target == null) return;
            btnCancelMatch.Text = text;
            btnCancelMatch.Size = new Size(target.Width, 42);
            btnCancelMatch.Location = new Point(target.Left, target.Bottom + 10);

            btnCancelMatch.Visible = true;
            btnCancelMatch.Enabled = true;
            btnCancelMatch.BringToFront();
            RoundControl(btnCancelMatch, 22);
        }

        private void ShowCancelOn(Control target, string text)
        {
            if (target == null) return;
            btnCancelMatch.Text = text;
            btnCancelMatch.Size = target.Size;
            btnCancelMatch.Location = target.Location;
            btnCancelMatch.Visible = true;
            btnCancelMatch.Enabled = true;
            btnCancelMatch.BringToFront();
            RoundControl(btnCancelMatch, 22);
        }

        private void SetMode(UIMode mode)
        {
            _mode = mode;
            switch (_mode)
            {
                case UIMode.Idle:
                    pnlCreatedId.Visible = false;
                    pnlJoinId.Visible = false;
                    btnCancelMatch.Visible = false;
                    btnCancelMatch.Enabled = false;
                    break;

                case UIMode.Joining:
                    pnlCreatedId.Visible = false;
                    pnlJoinId.Visible = true;
                    pnlJoinId.BringToFront();
                    ShowCancelBelow(pnlJoinId, "Hủy tìm phòng");
                    break;

                case UIMode.Matching:
                    pnlCreatedId.Visible = false;
                    pnlJoinId.Visible = false;
                    ShowCancelOn(pnlJoinId, "Hủy ghép");
                    break;

                case UIMode.Creating:
                    pnlCreatedId.Visible = true;
                    pnlJoinId.Visible = false;
                    pnlCreatedId.BringToFront();
                    ShowCancelBelow(pnlCreatedId, "Hủy phòng");
                    break;
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClientManager.Username))
            {
                MessageBox.Show("Chưa đăng nhập!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var win = new ChessUI.ProfileWindow(ClientManager.Username);
                try { var helper = new System.Windows.Interop.WindowInteropHelper(win); helper.Owner = this.Handle; } catch { }
                win.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show("Không thể mở hồ sơ. Lỗi: " + ex.Message); }
        }

        private void btnFriend_Click(object sender, EventArgs e)
        {
            try { Friend frm = new Friend(); frm.StartPosition = FormStartPosition.CenterParent; frm.ShowDialog(this); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            SetMode(UIMode.Matching);
            await SendRequest("FIND_GAME", "Đang tìm đối thủ.", button1);
        }

        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            SetMode(UIMode.Creating);
            txtCreatedRoomId.Text = "Đang tạo...";
            labelRoom.Text = "";
            await SendRequest("CREATE_ROOM", "Đang tạo phòng.", btnCreateRoom);
        }

        private async void btnJoinRoom_Click(object sender, EventArgs e)
        {
            SetMode(UIMode.Joining);
            string roomId = txtRoomId.Text.Trim();
            if (string.IsNullOrEmpty(roomId) || roomId == PLACEHOLDER_JOIN) return;
            await SendRequest($"JOIN_ROOM|{roomId}", "Đang vào phòng...", btnJoinRoom);
        }

        private void btnCancelMatch_Click(object sender, EventArgs e)
        {
            _isListening = false;
            CloseMatchFormIfAny();
            ResetUI();
        }

        private async Task SendRequest(string command, string waitText, Button clickedButton)
        {
            if (_isListening) return;
            if (!ClientManager.Instance.IsConnected)
            {
                MessageBox.Show("Mất kết nối tới máy chủ!");
                return;
            }
            try
            {
                _isListening = true;
                button1.Enabled = false;
                btnCreateRoom.Enabled = false;
                btnJoinRoom.Enabled = false;
                if (btnCancelMatch.Visible) btnCancelMatch.Enabled = true;
                clickedButton.Text = waitText;
                await ClientManager.Instance.SendAsync(command);
                await ListenForMessages();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); ResetUI(); }
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
                            _isListening = false;
                            if (_isLoggingOut) return;
                            this.Invoke((MethodInvoker)(() => { MessageBox.Show("Mất kết nối!"); CloseMatchFormIfAny(); ResetUI(); }));
                            return;
                        }

                        if (message.StartsWith("GAME_START"))
                        {
                            _isListening = false;
                            this.Invoke((MethodInvoker)(() => { CloseMatchFormIfAny(); _allowCloseProgrammatically = true; this.Hide(); LaunchWpfGameWindow(message); }));
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
                                form.Location = new Point(this.Location.X + (this.Width - form.Width) / 2, this.Location.Y + (this.Height - form.Height) / 2);
                                form.Accepted += () => { _ = ClientManager.Instance.SendAsync($"MATCH_RESPONSE|{roomId}|ACCEPT"); };
                                form.Declined += () => { _ = ClientManager.Instance.SendAsync($"MATCH_RESPONSE|{roomId}|DECLINE"); _isListening = false; ResetUI(); _currentMatchForm = null; };
                                form.FormClosed += (s, ev) => { if (_currentMatchForm == form) _currentMatchForm = null; };
                                _currentMatchForm = form;
                                form.Show(this);
                            }));
                        }
                        else if (message.StartsWith("MATCH_CANCELLED"))
                        {
                            _isListening = false;
                            this.Invoke((MethodInvoker)(() => { CloseMatchFormIfAny(); MessageBox.Show("Hủy trận."); ResetUI(); }));
                            return;
                        }
                        else if (message.StartsWith("ROOM_CREATED"))
                        {
                            string[] parts = message.Split('|');
                            string id = parts.Length > 1 ? parts[1] : "";
                            this.Invoke((MethodInvoker)(() =>
                            {
                                SetMode(UIMode.Creating);
                                txtCreatedRoomId.Text = "ID: " + id;
                                btnCreateRoom.Text = "Đang chờ...";
                            }));
                        }
                        else if (message.StartsWith("ERROR"))
                        {
                            _isListening = false;
                            this.Invoke((MethodInvoker)(() => { CloseMatchFormIfAny(); MessageBox.Show(message); ResetUI(); }));
                            return;
                        }
                        else if (message.StartsWith("WAITING"))
                        {
                            this.Invoke((MethodInvoker)(() => { button1.Text = "Đang đợi..."; SetMode(UIMode.Matching); }));
                        }
                    }
                    catch { _isListening = false; if (_isLoggingOut) return; this.Invoke((MethodInvoker)(() => { CloseMatchFormIfAny(); ResetUI(); })); return; }
                }
            });
        }

        private void CloseMatchFormIfAny()
        {
            try { if (_currentMatchForm != null && !_currentMatchForm.IsDisposed) { _currentMatchForm.Close(); _currentMatchForm = null; } } catch { }
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
                        if (!this.IsDisposed) this.Invoke((MethodInvoker)(() => { this.Show(); this.WindowState = FormWindowState.Normal; this.BringToFront(); ResetUI(); }));
                    };
                    gameWindow.Show();
                    Dispatcher.Run();
                }
                catch (Exception ex) { this.Invoke((MethodInvoker)(() => { MessageBox.Show("Lỗi game: " + ex.Message); ResetUI(); this.Show(); })); }
            });
            wpfThread.SetApartmentState(ApartmentState.STA);
            wpfThread.Start();
        }

        private void ResetUI()
        {
            _isListening = false;
            if (button1.InvokeRequired) { this.Invoke((MethodInvoker)ResetUI); return; }

            button1.Text = "Ghép Trận Ngẫu Nhiên";
            btnCreateRoom.Text = "Tạo Phòng";
            btnJoinRoom.Text = "Tìm / Vào Phòng";

            button1.Enabled = true;
            btnCreateRoom.Enabled = true;
            btnJoinRoom.Enabled = true;
            btnProfile.Enabled = true;

            labelRoom.Text = "";
            txtCreatedRoomId.Text = "";
            SetMode(UIMode.Idle);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ask = MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ask != DialogResult.Yes) return;
            _isLoggingOut = true;
            _isListening = false;
            _allowCloseProgrammatically = true;
            CloseMatchFormIfAny();
            try { ClientManager.Disconnect(); } catch { }
            this.Hide();
            using (var login = new Login()) { login.StartPosition = FormStartPosition.CenterScreen; login.ShowDialog(); }
            this.Close();
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !_allowCloseProgrammatically)
            {
                e.Cancel = true;
                MessageBox.Show("Hãy dùng nút Đăng xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PicLeaderboard_Click(object sender, EventArgs e)
        {
            try
            {
                var win = new ChessUI.LeaderboardWindow(); // WPF window bạn đã tạo
                try
                {
                    // set owner để nó đứng trước MainMenu (không bắt buộc)
                    var helper = new System.Windows.Interop.WindowInteropHelper(win);
                    helper.Owner = this.Handle;
                }
                catch { }

                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở Leaderboard. Lỗi: " + ex.Message);
            }
        }
    }
}