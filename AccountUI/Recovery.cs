using System;
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
        }

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
