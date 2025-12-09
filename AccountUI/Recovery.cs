using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace AccountUI
{
    public partial class Recovery : Form
    {
        private string otpCode = "";
        public static string selectedEmail = "";   // ⬅ Lưu email để ResetPassword dùng lại

        private string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=ChessDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public Recovery()
        {
            InitializeComponent();

            // Bo góc + canh giữa card giống Signup
            Load += Recovery_Load;
            Resize += Recovery_Resize;
        }

        // ================== UI: BO GÓC + CANH GIỮA CARD ==================
        private void Recovery_Load(object sender, EventArgs e)
        {
            CenterCard();

            // Bo góc card và các nút giống style Signup
            MakeRounded(panelCard, 26);
            MakeRounded(btnGui, 20);
            MakeRounded(btnXacNhan, 20);
            MakeRounded(btnQuayLai, 18);
        }

        private void Recovery_Resize(object sender, EventArgs e)
        {
            CenterCard();
        }

        private void CenterCard()
        {
            if (panelCard == null) return;

            panelCard.Left = (ClientSize.Width - panelCard.Width) / 2;
            panelCard.Top = (ClientSize.Height - panelCard.Height) / 2;
        }

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

        // ======================= LOGIC GỐC =======================

        // Kiểm tra email có trong DB
        private bool CheckEmailInDB(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT COUNT(*) FROM Users WHERE Email = @e";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Không thể kết nối database!");
                return false;
            }
        }

        // Gửi OTP
        private async void btnGui_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (email == "" || email == "Email")
            {
                MessageBox.Show("Vui lòng nhập Email!");
                return;
            }

            // Check email trong DB
            if (!CheckEmailInDB(email))
            {
                MessageBox.Show("Email này không tồn tại trong hệ thống!");
                return;
            }

            // Tạo OTP
            Random rd = new Random();
            otpCode = rd.Next(100000, 999999).ToString();

            string subject = "Mã Khôi Phục Mật Khẩu - CHESS ONLINE";
            string body = $"Mã OTP của bạn là: {otpCode}";

            bool sent = await EmailService.SendEmailAsync(email, subject, body);

            if (sent)
            {
                MessageBox.Show("Đã gửi OTP, hãy kiểm tra email!");

                selectedEmail = email;   // ⬅ Ghi nhớ email để reset mật khẩu
            }
            else
            {
                MessageBox.Show("Gửi email thất bại. Kiểm tra App Password Gmail!");
            }
        }

        // Xác nhận OTP
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (txtOTP.Text.Trim() == otpCode)
            {
                MessageBox.Show("OTP chính xác! Hãy đặt mật khẩu mới.");

                Resetpassword f = new Resetpassword();
                f.ShowDialog();

                this.Close();
            }
            else
            {
                MessageBox.Show("Sai OTP!");
            }
        }

        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
