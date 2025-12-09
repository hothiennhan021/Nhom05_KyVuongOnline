using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AccountUI.Properties;
using ChessClient;   // để dùng ClientSocket

namespace AccountUI
{
    public partial class Signup : Form
    {
        private bool _otpVerified = false;

        public Signup()
        {
            InitializeComponent();
            LoadImagesFromResources();
            CenterCard();
        }

        // ===============================================================
        //                 LOAD ẢNH TỪ RESOURCES
        // ===============================================================
        private void LoadImagesFromResources()
        {
            try
            {
                // Background
                this.BackgroundImage = Resources.Bg;
                this.BackgroundImageLayout = ImageLayout.Stretch;

                // Logo knight
                logoPictureBox.Image = Resources.icon_knight;

                // Icon mắt password
                button_passwordshow.Image = Resources.icon_eye_open;
                button_passwordshow2.Image = Resources.icon_eye_open;
                button_passwordhide.Image = Resources.icon_eye_hidden;
                button_passwordhide2.Image = Resources.icon_eye_hidden;
            }
            catch
            {
                // nếu thiếu resource thì bỏ qua, tránh crash
            }
        }

        // ===============================================================
        //                      FORM LOAD
        // ===============================================================
        private void Signup_Load(object sender, EventArgs e)
        {
            // Bo góc card + các panel input
            MakeRounded(cardPanel, 28);

            MakeRounded(panelUsername, 16);
            MakeRounded(panelEmail, 16);
            MakeRounded(panelOtp, 16);
            MakeRounded(panelFullName, 16);
            MakeRounded(panelBirthday, 16);
            MakeRounded(panelPassword, 16);
            MakeRounded(panelConfirmPassword, 16);

            // Nút
            MakeRounded(btnSignup, 22);
            MakeRounded(button2, 18);
            MakeRounded(btnSendOtp, 16);
            MakeRounded(btnVerifyOtp, 16);

            // ===========================================================
            //       CĂN LOGO CHESS ONLINE GIỮA CARD (GIỐNG LOGIN)
            // ===========================================================
            int iconSize = 72;

            // ô vuông knight
            panelKnightBg.Size = new Size(iconSize, iconSize);

            logoPictureBox.Dock = DockStyle.Fill;
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            // chữ CHESS / ONLINE
            labelBrand.Font = new Font("Segoe UI", 20f, FontStyle.Bold);
            labelBrandSub.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            labelBrand.AutoSize = true;
            labelBrandSub.AutoSize = true;

            int gapHorizontal = 12; // khoảng cách icon <-> CHESS
            int gapVertical = 2;  // CHESS <-> ONLINE

            int textWidth = Math.Max(labelBrand.Width, labelBrandSub.Width);
            int textHeight = labelBrand.Height + gapVertical + labelBrandSub.Height;

            int blockWidth = iconSize + gapHorizontal + textWidth;
            int blockHeight = Math.Max(iconSize, textHeight);

            // panelHeader chứa cả logo + text
            panelHeader.Size = new Size(blockWidth, blockHeight);
            panelHeader.Left = (cardPanel.Width - panelHeader.Width) / 2;
            panelHeader.Top = 40; // cách mép trên card một đoạn giống login

            // canh ô knight theo chiều dọc trong header
            panelKnightBg.Location = new Point(
                0,
                (panelHeader.Height - panelKnightBg.Height) / 2
            );

            // canh chữ bên phải icon
            int textStartX = panelKnightBg.Right + gapHorizontal;
            int textStartY = (panelHeader.Height - textHeight) / 2;

            labelBrand.Location = new Point(textStartX, textStartY);
            labelBrandSub.Location = new Point(
                textStartX + (labelBrand.Width - labelBrandSub.Width) / 2,
                labelBrand.Bottom + gapVertical
            );

            // Tiêu đề "Register" nằm dưới logo, căn giữa
            labelTitle.Top = panelHeader.Bottom + 30;
            labelTitle.Left = 0;
            labelTitle.Width = cardPanel.Width;
        }

        // ===============================================================
        //                      CENTER CARD
        // ===============================================================
        private void Signup_Resize(object sender, EventArgs e)
        {
            CenterCard();
        }

        private void CenterCard()
        {
            if (cardPanel == null) return;

            cardPanel.Left = (ClientSize.Width - cardPanel.Width) / 2;
            cardPanel.Top = (ClientSize.Height - cardPanel.Height) / 2;
        }

        // ===============================================================
        //                     HÀM BO GÓC CONTROL
        // ===============================================================
        private void MakeRounded(Control c, int radius)
        {
            if (c == null || c.Width <= 0 || c.Height <= 0) return;

            Rectangle rect = new Rectangle(0, 0, c.Width, c.Height);
            using (GraphicsPath path = RoundedRect(rect, radius))
            {
                c.Region = new Region(path);
            }
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

        // ===============================================================
        //                 NHẬP USERNAME – EMAIL – FULLNAME
        // ===============================================================
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

        // ===============================================================
        //                     PASSWORD + CONFIRM PASSWORD
        // ===============================================================
        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Mật Khẩu")
            {
                textBox3.Text = "";
                textBox3.PasswordChar = '*';
                textBox3.ForeColor = Color.White;
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                textBox3.Text = "Mật Khẩu";
                textBox3.PasswordChar = '\0';
                textBox3.ForeColor = Color.FromArgb(180, 182, 196);
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "Xác Nhận Mật Khẩu")
            {
                textBox4.Text = "";
                textBox4.PasswordChar = '*';
                textBox4.ForeColor = Color.White;
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                textBox4.Text = "Xác Nhận Mật Khẩu";
                textBox4.PasswordChar = '\0';
                textBox4.ForeColor = Color.FromArgb(180, 182, 196);
            }
        }

        // ===============================================================
        //                 SHOW / HIDE PASSWORD
        // ===============================================================
        private void button_passwordshow_Click(object sender, EventArgs e)
        {
            if (textBox3.PasswordChar == '*')
            {
                textBox3.PasswordChar = '\0';
                button_passwordhide.Visible = true;
                button_passwordhide.BringToFront();
            }
        }

        private void button_passwordhide_Click(object sender, EventArgs e)
        {
            if (textBox3.PasswordChar == '\0')
            {
                textBox3.PasswordChar = '*';
                button_passwordshow.BringToFront();
                button_passwordhide.Visible = false;
            }
        }

        private void button_passwordshow2_Click(object sender, EventArgs e)
        {
            if (textBox4.PasswordChar == '*')
            {
                textBox4.PasswordChar = '\0';
                button_passwordhide2.Visible = true;
                button_passwordhide2.BringToFront();
            }
        }

        private void button_passwordhide2_Click(object sender, EventArgs e)
        {
            if (textBox4.PasswordChar == '\0')
            {
                textBox4.PasswordChar = '*';
                button_passwordshow2.BringToFront();
                button_passwordhide2.Visible = false;
            }
        }

        // ===============================================================
        //                     OTP INPUT
        // ===============================================================
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

        // ===============================================================
        //                         GỬI OTP (logic gốc)
        // ===============================================================
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

        // ===============================================================
        //                      XÁC MINH OTP (logic gốc)
        // ===============================================================
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

        // ===============================================================
        //                       NÚT ĐĂNG KÝ (REGISTER)
        // ===============================================================
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

            // giữ đúng chuỗi REGISTER gốc
            string req = $"REGISTER|{username}|{pass}|{email}|{fullname}|{birthday}";
            string res = ClientSocket.SendAndReceive(req);

            if (res.StartsWith("REGISTER_SUCCESS"))
            {
                MessageBox.Show("Đăng ký thành công!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi đăng ký: " + res);
            }
        }

        // QUAY LẠI
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
