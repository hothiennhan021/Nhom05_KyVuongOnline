using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessClient;

namespace AccountUI
{
    public partial class Login : Form
    {
        private bool showPass = false;

        public Login()
        {
            InitializeComponent();
            Load += Login_Load;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            // ===== ICON / RESOURCE =====
            picKnight.Image = Properties.Resources.icon_knight;
            picUser.Image = Properties.Resources.icon_user;
            picLock.Image = Properties.Resources.icon_lock;
            picEye.Image = Properties.Resources.icon_eye_open;

            // ===== PLACEHOLDER MẶC ĐỊNH =====
            txtUser.Text = "Username";
            txtUser.ForeColor = Color.Gray;

            txtPass.Text = "Password";
            txtPass.ForeColor = Color.Gray;
            txtPass.UseSystemPasswordChar = false;

            // =====================================================================
            //  LOGO: Ô VUÔNG KNIGHT + CHESS / ONLINE – TO, GIỮA, CÂN ĐẸP NHƯ MẪU
            // =====================================================================

            int iconSize = 72;
            panelKnightBg.Size = new Size(iconSize, iconSize);

            picKnight.Dock = DockStyle.Fill;
            picKnight.SizeMode = PictureBoxSizeMode.Zoom;

            lblChess.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
            lblOnline.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            lblChess.AutoSize = true;
            lblOnline.AutoSize = true;

            int gapHorizontal = 12;   // icon <-> CHESS
            int gapVertical = 2;    // CHESS <-> ONLINE

            int textWidth = Math.Max(lblChess.Width, lblOnline.Width);
            int textHeight = lblChess.Height + gapVertical + lblOnline.Height;

            int blockWidth = iconSize + gapHorizontal + textWidth;
            int blockHeight = Math.Max(iconSize, textHeight);

            panelIcon.Size = new Size(blockWidth, blockHeight);
            panelIcon.Left = (panelCard.Width - panelIcon.Width) / 2;

            panelKnightBg.Location = new Point(
                0,
                (panelIcon.Height - panelKnightBg.Height) / 2
            );

            int textStartX = panelKnightBg.Right + gapHorizontal;
            int textStartY = (panelIcon.Height - textHeight) / 2;

            lblChess.Location = new Point(textStartX, textStartY);
            lblOnline.Location = new Point(
                textStartX + (lblChess.Width - lblOnline.Width) / 2,
                lblChess.Bottom + gapVertical
            );

            // =====================================================================
            //  CĂN GIỮA CÁC PHẦN CÒN LẠI TRÊN CARD
            // =====================================================================

            lblWelcome.AutoSize = true;
            linkForgot.AutoSize = true;
            linkCreate.AutoSize = true;

            lblWelcome.Left = (panelCard.Width - lblWelcome.Width) / 2;
            pnlUser.Left = (panelCard.Width - pnlUser.Width) / 2;
            pnlPass.Left = (panelCard.Width - pnlPass.Width) / 2;
            btnLogin.Left = (panelCard.Width - btnLogin.Width) / 2;
            linkForgot.Left = (panelCard.Width - linkForgot.Width) / 2;
            linkCreate.Left = (panelCard.Width - linkCreate.Width) / 2;

            // =====================================================================
            //  PLACEHOLDER USER / PASSWORD
            // =====================================================================

            txtUser.GotFocus += (s, _) =>
            {
                if (txtUser.Text == "Username")
                {
                    txtUser.Text = "";
                    txtUser.ForeColor = Color.White;
                }
            };
            txtUser.LostFocus += (s, _) =>
            {
                if (string.IsNullOrWhiteSpace(txtUser.Text))
                {
                    txtUser.Text = "Username";
                    txtUser.ForeColor = Color.Gray;
                }
            };

            txtPass.GotFocus += (s, _) =>
            {
                if (txtPass.Text == "Password")
                {
                    txtPass.Text = "";
                    txtPass.ForeColor = Color.White;
                    txtPass.UseSystemPasswordChar = true;
                }
            };
            txtPass.LostFocus += (s, _) =>
            {
                if (string.IsNullOrWhiteSpace(txtPass.Text))
                {
                    txtPass.UseSystemPasswordChar = false;
                    txtPass.Text = "Password";
                    txtPass.ForeColor = Color.Gray;

                    showPass = false;
                    picEye.Image = Properties.Resources.icon_eye_open;
                }
            };

            // =====================================================================
            //  TOGGLE ICON MẮT
            // =====================================================================

            picEye.Click += (s, _) =>
            {
                if (txtPass.Text == "Password")
                    return;

                showPass = !showPass;
                txtPass.UseSystemPasswordChar = !showPass;
                picEye.Image = showPass
                    ? Properties.Resources.icon_eye_hidden
                    : Properties.Resources.icon_eye_open;
            };

            // =====================================================================
            //  NÚT LOGIN – LOGIC GIỐNG FILE GỐC (CHỈ ĐỔI TÊN CONTROL)
            // =====================================================================

            btnLogin.Click += async (s, _) => await DoLogin();

            RoundControl(btnLogin, 24);
        }

        private void RoundControl(Control control, int radius)
        {
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

        // ====== LOGIC ĐĂNG NHẬP TỪ FILE GỐC (PORT LẠI SANG txtUser / txtPass / btnLogin) ======
        private async Task DoLogin()
        {
            string tentk = txtUser.Text.Trim();
            string matkhau = txtPass.Text; // giữ nguyên như file gốc

            // giống file gốc: chỉ check username (nhưng dùng placeholder mới "Username")
            if (string.IsNullOrWhiteSpace(tentk) || tentk == "Username")
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!");
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Đang xử lý...";

            try
            {
                await ClientManager.ConnectToServerAsync("127.0.0.1", 8888);

                string request = $"LOGIN|{tentk}|{matkhau}";
                await ClientManager.Instance.SendAsync(request);

                string response = await Task.Run(() => ClientManager.Instance.WaitForMessage());

                if (string.IsNullOrEmpty(response))
                {
                    MessageBox.Show("Lỗi: Server không phản hồi hoặc mất kết nối.");
                    btnLogin.Enabled = true;
                    btnLogin.Text = "LOGIN";
                    return;
                }

                var parts = response.Split('|');
                var command = parts[0];

                if (command == "LOGIN_SUCCESS")
                {
                    ClientManager.Username = tentk;

                    try
                    {
                        bool connected = ClientSocket.Connect("127.0.0.1", 8888);
                        if (connected)
                        {
                            ClientSocket.SendAndReceive($"LOGIN|{tentk}|{matkhau}");
                        }
                    }
                    catch { }

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo");
                    this.Hide();
                    using (var mainmenu = new MainMenu())
                    {
                        mainmenu.ShowDialog();
                    }
                    this.Close();
                }
                else
                {
                    string msg = parts.Length > 1 ? parts[1] : "Lỗi đăng nhập";
                    MessageBox.Show(msg);
                    ClientManager.Disconnect();
                    btnLogin.Enabled = true;
                    btnLogin.Text = "LOGIN";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
                btnLogin.Enabled = true;
                btnLogin.Text = "LOGIN";
            }
        }

        // 2 link mở form giống file gốc (chỉ khác tên biến)
        private void linkForgot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var dlg = new Recovery())
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ShowDialog(this);
            }
        }

        private void linkCreate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var dlg = new Signup())
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ShowDialog(this);
            }
        }
    }
}
