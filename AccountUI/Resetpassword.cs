using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AccountUI
{
    public partial class Resetpassword : Form
    {
        public Resetpassword()
        {
            InitializeComponent();

            // --- BỔ SUNG: Cài đặt hình nền an toàn ở đây ---
            // Code này chạy khi form khởi tạo, không ảnh hưởng đến Designer
            try
            {
                this.BackgroundImage = Properties.Resources.Bg;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { }
            // -----------------------------------------------

            Load += Resetpassword_Load;
            Resize += Resetpassword_Resize;
        }

        private void Resetpassword_Load(object sender, EventArgs e)
        {
            CenterCard();
            MakeRounded(panelCard, 26);
            MakeRounded(btnXacNhan, 20);
            MakeRounded(btnQuayLai, 18);
        }

        private void Resetpassword_Resize(object sender, EventArgs e)
        {
            CenterCard();
        }

        private void CenterCard()
        {
            panelCard.Left = (ClientSize.Width - panelCard.Width) / 2;
            panelCard.Top = (ClientSize.Height - panelCard.Height) / 2;
        }

        private void MakeRounded(Control c, int radius)
        {
            Rectangle rect = new Rectangle(0, 0, c.Width, c.Height);
            using GraphicsPath path = RoundedRect(rect, radius);
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

        // ================= RESET PASSWORD =================

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            string pass1 = txtPass1.Text.Trim();
            string pass2 = txtPass2.Text.Trim();

            if (pass1 == "" || pass2 == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pass1.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có tối thiểu 6 ký tự!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pass1 != pass2)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mã hóa mật khẩu trước khi gửi (Sử dụng BCrypt)
            // Lưu ý: Cần cài gói NuGet BCrypt.Net-Next nếu chưa có
            string hash = "";
            try
            {
                hash = BCrypt.Net.BCrypt.HashPassword(pass1);
            }
            catch
            {
                // Fallback nếu chưa cài BCrypt, gửi tạm plain text (không khuyến khích)
                hash = pass1;
            }

            string res = ClientSocket.SendAndReceive(
                $"FORGOT_RESET_PASSWORD|{Recovery.selectedEmail}|{hash}");

            if (res == "OK")
            {
                MessageBox.Show("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi: " + res, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}