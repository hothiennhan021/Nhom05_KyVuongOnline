using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AccountUI
{
    public partial class Recovery : Form
    {
        public static string selectedEmail = "";

        public Recovery()
        {
            InitializeComponent();
            Load += Recovery_Load;
            Resize += Recovery_Resize;
        }

        private void Recovery_Load(object sender, EventArgs e)
        {
            CenterCard();
            MakeRounded(panelCard, 26);
            MakeRounded(btnGui, 20);
            MakeRounded(btnXacNhan, 20);
            // Không cần bo tròn nút text "Quay lại" quá nhiều, nhưng giữ nguyên theo ý bạn
            MakeRounded(btnQuayLai, 18);
        }

        private void Recovery_Resize(object sender, EventArgs e)
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

        // ================= QUÊN MẬT KHẨU =================

        private void btnGui_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            if (email == "")
            {
                MessageBox.Show("Vui lòng nhập Email!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gửi yêu cầu lên Server
            string res = ClientSocket.SendAndReceive($"FORGOT_SEND_OTP|{email}");

            if (string.IsNullOrWhiteSpace(res))
            {
                MessageBox.Show("Không nhận được phản hồi từ Server!", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (res.StartsWith("OK"))
            {
                selectedEmail = email;
                MessageBox.Show("Đã gửi mã OTP, vui lòng kiểm tra hộp thư email của bạn!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtOTP.Focus(); // Chuyển con trỏ xuống ô OTP cho tiện
            }
            else if (res.StartsWith("ERROR|"))
            {
                MessageBox.Show(res.Substring(6), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra, vui lòng thử lại sau!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            string otp = txtOTP.Text.Trim();
            if (string.IsNullOrEmpty(otp))
            {
                MessageBox.Show("Vui lòng nhập mã OTP!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string res = ClientSocket.SendAndReceive($"FORGOT_VERIFY_OTP|{selectedEmail}|{otp}");

            if (res == "OK")
            {
                MessageBox.Show("Xác thực OTP thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Mở form đặt lại mật khẩu
                new Resetpassword().ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Mã OTP không chính xác hoặc đã hết hạn.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}