using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using BCrypt.Net;

namespace AccountUI
{
    public partial class Resetpassword : Form
    {
        private string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=ChessDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public Resetpassword()
        {
            InitializeComponent();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            string pass1 = txtPass1.Text.Trim();
            string pass2 = txtPass2.Text.Trim();

            if (pass1 == "" || pass2 == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu!");
                return;
            }

            if (pass1.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải tối thiểu 6 ký tự!");
                return;
            }

            if (pass1 != pass2)
            {
                MessageBox.Show("Xác nhận mật khẩu không khớp!");
                return;
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(pass1);

            if (UpdatePassword(Recovery.selectedEmail, hash))
            {
                MessageBox.Show("Đổi mật khẩu thành công!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Không thể cập nhật mật khẩu (Email không tồn tại hoặc lỗi kết nối).");
            }
        }

        private bool UpdatePassword(string email, string hash)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "UPDATE Users SET PasswordHash = @p WHERE Email = @e";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@p", hash);
                        cmd.Parameters.AddWithValue("@e", email);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
