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

        // ĐỊNH NGHĨA MÀU SẮC CHO DỄ NHÌN
        private Color colorPlaceholder = Color.Silver; // Màu xám sáng (cho chữ gợi ý)
        private Color colorText = Color.White;         // Màu trắng (cho chữ khi nhập)

        // HẰNG SỐ CHUỖI ĐỂ TRÁNH GÕ SAI
        private const string STR_TK = "Tên tài khoản";
        private const string STR_MK = "Mật khẩu";

        public Login()
        {
            InitializeComponent();
            this.Load += Login_Load;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            // ===== TÀI NGUYÊN BIỂU TƯỢNG =====
            picKnight.Image = Properties.Resources.icon_knight;
            picUser.Image = Properties.Resources.icon_user;
            picLock.Image = Properties.Resources.icon_lock;
            picEye.Image = Properties.Resources.icon_eye_open;

            // ===== THIẾT LẬP GIAO DIỆN GỢI Ý BAN ĐẦU =====

            // 1. Tên tài khoản
            txtUser.Text = STR_TK;
            txtUser.ForeColor = colorPlaceholder; // Dùng màu sáng hơn

            // 2. Mật khẩu 
            txtPass.Text = STR_MK;
            txtPass.ForeColor = colorPlaceholder; // Dùng màu sáng hơn

            // QUAN TRỌNG: Để hiện chữ "Mật khẩu" rõ ràng (không bị mã hóa thành dấu sao)
            txtPass.PasswordChar = '\0';
            txtPass.UseSystemPasswordChar = false;

            // ===== CÁC LOGIC VẼ GIAO DIỆN KHÁC =====
            SetupLayout();
            SetupEvents();
        }

        private void SetupEvents()
        {
            // --- XỬ LÝ SỰ KIỆN NHẬP TÊN TÀI KHOẢN ---
            txtUser.Enter += (s, e) =>
            {
                if (txtUser.Text == STR_TK)
                {
                    txtUser.Text = "";
                    txtUser.ForeColor = colorText; // Chuyển sang màu Trắng khi nhập
                }
            };

            txtUser.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtUser.Text))
                {
                    txtUser.Text = STR_TK;
                    txtUser.ForeColor = colorPlaceholder; // Về màu Xám sáng khi trống
                }
            };

            // --- XỬ LÝ SỰ KIỆN NHẬP MẬT KHẨU ---
            txtPass.Enter += (s, e) =>
            {
                if (txtPass.Text == STR_MK)
                {
                    txtPass.Text = "";
                    txtPass.ForeColor = colorText; // Chuyển sang màu Trắng

                    // Kích hoạt ẩn mật khẩu nếu chưa bấm hiện
                    if (!showPass)
                    {
                        txtPass.UseSystemPasswordChar = true;
                    }
                }
            };

            txtPass.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPass.Text))
                {
                    // Reset về trạng thái Gợi ý
                    txtPass.UseSystemPasswordChar = false;
                    txtPass.PasswordChar = '\0'; // Xóa ký tự ẩn
                    txtPass.Text = STR_MK;
                    txtPass.ForeColor = colorPlaceholder; // Về màu Xám sáng

                    showPass = false;
                    picEye.Image = Properties.Resources.icon_eye_open;
                }
            };

            // --- XỬ LÝ BIỂU TƯỢNG MẮT (ẨN/HIỆN MẬT KHẨU) ---
            picEye.Click += (s, e) =>
            {
                if (txtPass.Text == STR_MK) return;

                showPass = !showPass;

                if (showPass)
                {
                    // Hiện mật khẩu
                    txtPass.UseSystemPasswordChar = false;
                    txtPass.PasswordChar = '\0';
                    picEye.Image = Properties.Resources.icon_eye_hidden;
                }
                else
                {
                    // Ẩn mật khẩu
                    txtPass.UseSystemPasswordChar = true;
                    picEye.Image = Properties.Resources.icon_eye_open;
                }
            };

            // --- NÚT ĐĂNG NHẬP ---
            btnLogin.Click += async (s, e) => await DoLogin();
            RoundControl(btnLogin, 24);
        }

        // =====================================================================
        //  LOGIC ĐĂNG NHẬP
        // =====================================================================
        private async Task DoLogin()
        {
            string tentk = txtUser.Text.Trim();
            string matkhau = txtPass.Text;

            if (string.IsNullOrWhiteSpace(tentk) || tentk == STR_TK)
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(matkhau) || matkhau == STR_MK)
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Đang xử lý...";

            try
            {
                // Giữ nguyên Server theo yêu cầu
                await ClientManager.ConnectToServerAsync("127.0.0.1", 8888);

                // Lưu ý: Chuỗi gửi đi (LOGIN) là giao thức nên giữ nguyên tiếng Anh để Server hiểu
                string request = $"LOGIN|{tentk}|{matkhau}";
                await ClientManager.Instance.SendAsync(request);

                string response = await Task.Run(() => ClientManager.Instance.WaitForMessage());

                if (string.IsNullOrEmpty(response))
                {
                    MessageBox.Show("Server không phản hồi.", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetButtonLogin();
                    return;
                }

                var parts = response.Split('|');
                if (parts[0] == "LOGIN_SUCCESS")
                {
                    ClientManager.Username = tentk;
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
                    string msg = parts.Length > 1 ? parts[1] : "Sai tài khoản hoặc mật khẩu.";
                    MessageBox.Show(msg, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClientManager.Disconnect();
                    ResetButtonLogin();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetButtonLogin();
            }
        }

        private void ResetButtonLogin()
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "ĐĂNG NHẬP";
        }

        // =====================================================================
        //  HỖ TRỢ GIAO DIỆN (LAYOUT)
        // =====================================================================
        private void SetupLayout()
        {
            int iconSize = 72;
            panelKnightBg.Size = new Size(iconSize, iconSize);
            picKnight.Dock = DockStyle.Fill;
            picKnight.SizeMode = PictureBoxSizeMode.Zoom;

            // Cập nhật Font cho tên mới
            lblChess.Font = new Font("Segoe UI", 18f, FontStyle.Bold); // Giảm size chút vì chữ tiếng Việt dài hơn
            lblOnline.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            lblChess.AutoSize = true;
            lblOnline.AutoSize = true;

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

            lblWelcome.AutoSize = true;
            linkForgot.AutoSize = true;
            linkCreate.AutoSize = true;

            lblWelcome.Left = (panelCard.Width - lblWelcome.Width) / 2;
            pnlUser.Left = (panelCard.Width - pnlUser.Width) / 2;
            pnlPass.Left = (panelCard.Width - pnlPass.Width) / 2;
            btnLogin.Left = (panelCard.Width - btnLogin.Width) / 2;
            linkForgot.Left = (panelCard.Width - linkForgot.Width) / 2;
            linkCreate.Left = (panelCard.Width - linkCreate.Width) / 2;
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

        private void linkForgot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var dlg = new Recovery()) { dlg.StartPosition = FormStartPosition.CenterParent; dlg.ShowDialog(this); }
        }

        private void linkCreate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var dlg = new Signup()) { dlg.StartPosition = FormStartPosition.CenterParent; dlg.ShowDialog(this); }
        }
    }
}