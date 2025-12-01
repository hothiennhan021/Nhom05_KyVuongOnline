using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace AccountUI
{
    public partial class Signup : Form
    {
        private string generatedOtp = "";
        private bool isOtpVerified = false;

        public Signup()
        {
            InitializeComponent();

            // GÁN SỰ KIỆN (nếu bạn chưa gán trong designer)
            btnSendOtp.Click += btnSendOtp_Click;
            btnVerifyOtp.Click += btnVerifyOtp_Click;
        }

        // ============================================
        //  GỬI OTP
        // ============================================
        private async void btnSendOtp_Click(object sender, EventArgs e)
        {
            string email = textBox2.Text.Trim();

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9_.]{3,30}@gmail.com(.vn|)$"))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi");
                return;
            }

            // Tạo mã OTP ngẫu nhiên 6 số
            Random rd = new Random();
            generatedOtp = rd.Next(100000, 999999).ToString();
            isOtpVerified = false;

            string body = $"Mã OTP của bạn là: {generatedOtp}\nMã có hiệu lực trong 3 phút.";

            bool sent = await EmailService.SendEmailAsync(email, "Xác thực tài khoản", body);

            if (sent)
            {
                MessageBox.Show("OTP đã được gửi đến Gmail của bạn!", "Thông báo");
            }
            else
            {
                MessageBox.Show("Không gửi được OTP. Vui lòng thử lại!", "Lỗi");
            }
        }

        // ============================================
        //  XÁC MINH OTP
        // ============================================
        private void btnVerifyOtp_Click(object sender, EventArgs e)
        {
            if (txtOtp.Text.Trim() == generatedOtp && generatedOtp != "")
            {
                isOtpVerified = true;
                MessageBox.Show("Xác minh OTP thành công!", "Thành công");
            }
            else
            {
                isOtpVerified = false;
                MessageBox.Show("OTP sai, hãy thử lại!", "Lỗi");
            }
        }

        // ============================================
        //  NÚT ĐĂNG KÝ
        // ============================================
        private void button1_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu
            string tentk = textBox1.Text;
            string email = textBox2.Text;
            string matkhau = textBox3.Text;
            string xnmatkhau = textBox4.Text;
            string fullName = txtFullName.Text;
            string birthday = dtpBirthday.Value.ToString("yyyy-MM-dd");

            // Validate
            if (!Regex.IsMatch(tentk, @"^[A-Za-z0-9]{6,24}$") || tentk == "Tên Đăng Nhập")
            {
                MessageBox.Show("Tên đăng nhập phải dài 6-24 ký tự!", "Lỗi");
                return;
            }
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9_.]{3,30}@gmail.com(.vn|)$") || email == "Email")
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi");
                return;
            }
            if (fullName.Trim() == "" || fullName == "Họ và Tên")
            {
                MessageBox.Show("Vui lòng nhập họ và tên!", "Lỗi");
                return;
            }
            if (!Regex.IsMatch(matkhau, @"^[A-Za-z0-9]{6,24}$") || matkhau == "Mật Khẩu")
            {
                MessageBox.Show("Mật khẩu phải 6-24 ký tự!", "Lỗi");
                return;
            }
            if (xnmatkhau != matkhau)
            {
                MessageBox.Show("Xác nhận mật khẩu không khớp!", "Lỗi");
                return;
            }

            // ======= BẮT BUỘC PHẢI XÁC MINH OTP =======
            if (!isOtpVerified)
            {
                MessageBox.Show("Bạn phải xác minh OTP trước khi đăng ký!", "Lỗi");
                return;
            }

            try
            {
                // Gửi request đăng ký
                string request = $"REGISTER|{tentk}|{matkhau}|{email}|{fullName}|{birthday}";
                string response = ClientSocket.SendAndReceive(request);

                if (string.IsNullOrWhiteSpace(response))
                {
                    MessageBox.Show("Server không phản hồi!", "Lỗi");
                    return;
                }

                string raw = response.Trim();
                string[] parts = raw.Split('|');
                string command = parts[0].Trim().Replace("\uFEFF", "");

                if (command == "REGISTER_SUCCESS")
                {
                    string msg = (parts.Length > 1) ? parts[1] : "Đăng ký thành công!";
                    MessageBox.Show(msg, "Thông báo");
                    this.Close();
                }
                else if (command == "ERROR")
                {
                    string msg = (parts.Length > 1) ? parts[1] : "Đăng ký thất bại!";
                    MessageBox.Show(msg, "Lỗi");
                }
                else
                {
                    MessageBox.Show("Phản hồi không hợp lệ từ server:\n" + response, "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi server: " + ex.Message, "Lỗi");
            }
        }

        // ============================================
        //  CÁC HÀM CŨ — GIỮ NGUYÊN TOÀN BỘ
        // ============================================

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Quay Lại ?", "Chú Ý",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) this.Close();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Tên Đăng Nhập")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.DarkSlateBlue;
            }
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Tên Đăng Nhập";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Email")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.DarkSlateBlue;
            }
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Email";
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void TxtFullName_Enter(object sender, EventArgs e)
        {
            if (txtFullName.Text == "Họ và Tên")
            {
                txtFullName.Text = "";
                txtFullName.ForeColor = Color.DarkSlateBlue;
            }
        }
        private void TxtFullName_Leave(object sender, EventArgs e)
        {
            if (txtFullName.Text == "")
            {
                txtFullName.Text = "Họ và Tên";
                txtFullName.ForeColor = Color.Gray;
            }
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Mật Khẩu")
            {
                textBox3.Text = "";
                textBox3.ForeColor = Color.DarkSlateBlue;
                textBox3.PasswordChar = '*';
            }
        }
        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox3.Text = "Mật Khẩu";
                textBox3.ForeColor = Color.Gray;
                textBox3.PasswordChar = '\0';
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "Xác Nhận Mật Khẩu")
            {
                textBox4.Text = "";
                textBox4.ForeColor = Color.DarkSlateBlue;
                textBox4.PasswordChar = '*';
            }
        }
        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                textBox4.Text = "Xác Nhận Mật Khẩu";
                textBox4.ForeColor = Color.Gray;
                textBox4.PasswordChar = '\0';
            }
        }

        private void button_passwordhide_Click(object sender, EventArgs e)
        {
            if (textBox3.PasswordChar == '\0')
            {
                button_passwordshow.BringToFront();
                textBox3.PasswordChar = '*';
            }
        }
        private void button_passwordhide2_Click(object sender, EventArgs e)
        {
            if (textBox4.PasswordChar == '\0')
            {
                button_passwordshow2.BringToFront();
                textBox4.PasswordChar = '*';
            }
        }
        private void button_passwordshow_Click(object sender, EventArgs e)
        {
            if (textBox3.PasswordChar == '*')
            {
                button_passwordhide.BringToFront();
                textBox3.PasswordChar = '\0';
            }
        }
        private void button_passwordshow2_Click(object sender, EventArgs e)
        {
            if (textBox4.PasswordChar == '*')
            {
                button_passwordhide2.BringToFront();
                textBox4.PasswordChar = '\0';
            }
        }

        private void txtOtp_Enter(object sender, EventArgs e)
        {
            if (txtOtp.Text == "Nhập mã OTP")
            {
                txtOtp.Text = "";
                txtOtp.ForeColor = Color.DarkSlateBlue;
            }
        }

        private void txtOtp_Leave(object sender, EventArgs e)
        {
            if (txtOtp.Text == "")
            {
                txtOtp.Text = "Nhập mã OTP";
                txtOtp.ForeColor = Color.Gray;
            }
        }
    }
}
