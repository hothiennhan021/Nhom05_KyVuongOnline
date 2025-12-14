using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AccountUI.Properties;
using ChessClient;

namespace AccountUI
{
    public partial class Signup : Form
    {
        private bool _otpVerified = false;

        // trạng thái mắt
        private bool _showPass1 = false;
        private bool _showPass2 = false;

        public Signup()
        {
            InitializeComponent();
            CenterCard();
        }

        private void Signup_Load(object sender, EventArgs e)
        {
            // Rounded (giữ như style bạn đang dùng)
            MakeRounded(cardPanel, 28);

            MakeRounded(panelUsername, 16);
            MakeRounded(panelEmail, 16);
            MakeRounded(panelOtp, 16);
            MakeRounded(panelFullName, 16);
            MakeRounded(panelBirthday, 16);
            MakeRounded(panelPassword, 16);
            MakeRounded(panelConfirmPassword, 16);

            MakeRounded(btnSignup, 22);
            MakeRounded(button2, 18);
            MakeRounded(btnSendOtp, 16);
            MakeRounded(btnVerifyOtp, 16);

            // Background + logo (nếu có)
            try
            {
                if (Resources.Bg != null)
                {
                    BackgroundImage = Resources.Bg;
                    BackgroundImageLayout = ImageLayout.Stretch;
                }

                if (Resources.icon_knight != null)
                {
                    logoPictureBox.Image = Resources.icon_knight;
                    logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch { }

            // ✅ FIX ICON MẮT: dùng BackgroundImage + Zoom (không bị ô đen/chéo)
            try
            {
                StyleEyeButton(button_passwordshow, Resources.icon_eye_open);
                StyleEyeButton(button_passwordhide, Resources.icon_eye_hidden);
                StyleEyeButton(button_passwordshow2, Resources.icon_eye_open);
                StyleEyeButton(button_passwordhide2, Resources.icon_eye_hidden);
            }
            catch { }

            // trạng thái ban đầu
            button_passwordhide.Visible = false;
            button_passwordhide2.Visible = false;

            // đảm bảo nút mắt luôn nằm trên cùng
            button_passwordshow.BringToFront();
            button_passwordshow2.BringToFront();
            button_passwordhide.BringToFront();
            button_passwordhide2.BringToFront();

            _showPass1 = false;
            _showPass2 = false;

            // (tuỳ bạn) căn header giống Login — nếu layout đang ổn thì giữ
            try
            {
                int iconSize = 72;
                panelKnightBg.Size = new Size(iconSize, iconSize);

                labelBrand.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
                labelBrandSub.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
                labelBrand.AutoSize = true;
                labelBrandSub.AutoSize = true;

                int gapHorizontal = 12;
                int gapVertical = 2;

                int textWidth = Math.Max(labelBrand.Width, labelBrandSub.Width);
                int textHeight = labelBrand.Height + gapVertical + labelBrandSub.Height;

                int blockWidth = iconSize + gapHorizontal + textWidth;
                int blockHeight = Math.Max(iconSize, textHeight);

                panelHeader.Size = new Size(blockWidth, blockHeight);
                panelHeader.Left = (cardPanel.Width - panelHeader.Width) / 2;
                panelHeader.Top = 40;

                panelKnightBg.Location = new Point(0, (panelHeader.Height - panelKnightBg.Height) / 2);

                int textStartX = panelKnightBg.Right + gapHorizontal;
                int textStartY = (panelHeader.Height - textHeight) / 2;

                labelBrand.Location = new Point(textStartX, textStartY);
                labelBrandSub.Location = new Point(
                    textStartX + (labelBrand.Width - labelBrandSub.Width) / 2,
                    labelBrand.Bottom + gapVertical
                );

                labelTitle.Top = panelHeader.Bottom + 30;
                labelTitle.Left = 0;
                labelTitle.Width = cardPanel.Width;
            }
            catch { }
        }

        private void Signup_Resize(object sender, EventArgs e) => CenterCard();

        private void CenterCard()
        {
            if (cardPanel == null) return;
            cardPanel.Left = (ClientSize.Width - cardPanel.Width) / 2;
            cardPanel.Top = (ClientSize.Height - cardPanel.Height) / 2;
        }

        // ===================== STYLE BUTTON MẮT =====================
        private void StyleEyeButton(Button btn, Image icon)
        {
            btn.Text = "";
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;

            // nền đúng màu panel password
            btn.BackColor = Color.FromArgb(35, 45, 65);
            btn.FlatAppearance.MouseOverBackColor = btn.BackColor;
            btn.FlatAppearance.MouseDownBackColor = btn.BackColor;

            // ✅ dùng BackgroundImage để không bị lỗi render Image (ô đen/gạch)
            btn.Image = null;
            btn.BackgroundImage = icon;
            btn.BackgroundImageLayout = ImageLayout.Zoom;

            btn.Cursor = Cursors.Hand;
            btn.TabStop = false;

            // ✅ FIX: click mắt không gây validate / không kéo focus đi linh tinh
            btn.CausesValidation = false;
        }

        // ✅ helper: trả focus lại textbox khi bấm icon mắt
        private void KeepFocus(TextBox tb)
        {
            try
            {
                if (tb == null) return;
                tb.Focus();
                tb.SelectionStart = tb.TextLength;
                tb.SelectionLength = 0;
            }
            catch { }
        }

        // ===================== ROUNDED HELPER =====================
        private void MakeRounded(Control c, int radius)
        {
            if (c == null || c.Width <= 0 || c.Height <= 0) return;

            Rectangle rect = new Rectangle(0, 0, c.Width, c.Height);
            using (GraphicsPath path = RoundedRect(rect, radius))
                c.Region = new Region(path);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);

            path.CloseFigure();
            return path;
        }

        // ===================== PLACEHOLDER USERNAME =====================
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Tên Đăng Nhập")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.White;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Tên Đăng Nhập";
                textBox1.ForeColor = Color.FromArgb(180, 182, 196);
            }
        }

        // ===================== PLACEHOLDER EMAIL =====================
        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (txtEmail.Text == "Email")
            {
                txtEmail.Text = "";
                txtEmail.ForeColor = Color.White;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtEmail.Text = "Email";
                txtEmail.ForeColor = Color.FromArgb(180, 182, 196);
            }
        }

        // ===================== PLACEHOLDER FULLNAME =====================
        private void TxtFullName_Enter(object sender, EventArgs e)
        {
            if (txtFullName.Text == "Họ và Tên")
            {
                txtFullName.Text = "";
                txtFullName.ForeColor = Color.White;
            }
        }

        private void TxtFullName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                txtFullName.Text = "Họ và Tên";
                txtFullName.ForeColor = Color.FromArgb(180, 182, 196);
            }
        }

        // ===================== PASSWORD GIỐNG LOGIN =====================
        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Mật Khẩu")
            {
                textBox3.Text = "";
                textBox3.ForeColor = Color.White;
                textBox3.PasswordChar = _showPass1 ? '\0' : '*';
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                textBox3.Text = "Mật Khẩu";
                textBox3.PasswordChar = '\0';
                textBox3.ForeColor = Color.FromArgb(180, 182, 196);

                _showPass1 = false;
                button_passwordhide.Visible = false;
                button_passwordshow.BringToFront();
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "Xác Nhận Mật Khẩu")
            {
                textBox4.Text = "";
                textBox4.ForeColor = Color.White;
                textBox4.PasswordChar = _showPass2 ? '\0' : '*';
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                textBox4.Text = "Xác Nhận Mật Khẩu";
                textBox4.PasswordChar = '\0';
                textBox4.ForeColor = Color.FromArgb(180, 182, 196);

                _showPass2 = false;
                button_passwordhide2.Visible = false;
                button_passwordshow2.BringToFront();
            }
        }

        // ===================== CLICK MẮT =====================
        private void button_passwordshow_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "Mật Khẩu") return;

            _showPass1 = true;
            textBox3.PasswordChar = '\0';

            button_passwordhide.Visible = true;
            button_passwordhide.BringToFront();

            // ✅ FIX: không nhảy focus sang ô khác
            KeepFocus(textBox3);
        }

        private void button_passwordhide_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "Mật Khẩu") return;

            _showPass1 = false;
            textBox3.PasswordChar = '*';

            button_passwordhide.Visible = false;
            button_passwordshow.BringToFront();

            // ✅ FIX: không nhảy focus sang ô khác
            KeepFocus(textBox3);
        }

        private void button_passwordshow2_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "Xác Nhận Mật Khẩu") return;

            _showPass2 = true;
            textBox4.PasswordChar = '\0';

            button_passwordhide2.Visible = true;
            button_passwordhide2.BringToFront();

            // ✅ FIX: không nhảy focus sang ô khác
            KeepFocus(textBox4);
        }

        private void button_passwordhide2_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "Xác Nhận Mật Khẩu") return;

            _showPass2 = false;
            textBox4.PasswordChar = '*';

            button_passwordhide2.Visible = false;
            button_passwordshow2.BringToFront();

            // ✅ FIX: không nhảy focus sang ô khác
            KeepFocus(textBox4);
        }

        // ===================== OTP PLACEHOLDER =====================
        private void txtOtp_Enter(object sender, EventArgs e)
        {
            if (txtOtp.Text == "Nhập mã OTP")
            {
                txtOtp.Text = "";
                txtOtp.ForeColor = Color.White;
            }
        }

        private void txtOtp_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOtp.Text))
            {
                txtOtp.Text = "Nhập mã OTP";
                txtOtp.ForeColor = Color.FromArgb(180, 182, 196);
            }
        }

        // ===================== SEND OTP (giữ logic) =====================
        private void btnSendOtp_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (!Regex.IsMatch(email, @"^[^@\s]+@gmail\.com(\.vn)?$"))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi");
                return;
            }

            string res = ClientSocket.SendAndReceive($"REQUEST_OTP|{email}");

            if (res.StartsWith("OTP_SENT"))
            {
                MessageBox.Show("OTP đã được gửi vào email!");
                _otpVerified = false;
            }
            else
            {
                MessageBox.Show("Lỗi gửi OTP!");
            }
        }

        // ===================== VERIFY OTP (giữ logic) =====================
        private void btnVerifyOtp_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string otp = txtOtp.Text.Trim();

            if (otp == "" || otp == "Nhập mã OTP")
            {
                MessageBox.Show("Bạn chưa nhập OTP!");
                return;
            }

            string res = ClientSocket.SendAndReceive($"VERIFY_OTP|{email}|{otp}");

            if (res.StartsWith("OTP_OK"))
            {
                MessageBox.Show("Xác minh OTP thành công!");
                _otpVerified = true;
            }
            else
            {
                MessageBox.Show("OTP sai hoặc hết hạn!");
                _otpVerified = false;
            }
        }

        // ===================== REGISTER (giữ logic) =====================
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string email = txtEmail.Text.Trim();
            string pass = textBox3.Text.Trim();
            string pass2 = textBox4.Text.Trim();
            string fullname = txtFullName.Text.Trim();
            string birthday = dtpBirthday.Value.ToString("yyyy-MM-dd");

            if (!_otpVerified)
            {
                MessageBox.Show("Bạn phải xác minh OTP trước!");
                return;
            }

            // nếu muốn check confirm password:
            // if (pass != pass2) { MessageBox.Show("Mật khẩu xác nhận không khớp!"); return; }

            string req = $"REGISTER|{username}|{pass}|{email}|{fullname}|{birthday}";
            string res = ClientSocket.SendAndReceive(req);

            if (res.StartsWith("REGISTER_SUCCESS"))
            {
                MessageBox.Show("Đăng ký thành công!");
                Close();
            }
            else
            {
                MessageBox.Show("Lỗi đăng ký: " + res);
            }
        }

        private void button2_Click(object sender, EventArgs e) => Close();
    }
}
