using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

            // Bo góc + canh giữa card giống Signup / Recovery
            Load += Resetpassword_Load;
            Resize += Resetpassword_Resize;
        }

        // ================== UI: BO GÓC + CANH GIỮA CARD ==================
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
