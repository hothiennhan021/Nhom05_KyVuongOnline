using System.Drawing;
using System.Windows.Forms;

namespace AccountUI
{
    partial class Resetpassword
    {
        private System.ComponentModel.IContainer components = null;

        private RoundedPanel panelCard;
        private PictureBox pictureLogo;
        private Label lblAppName;
        private Label lblOnline; // <--- Đã thêm Label Online
        private Label lblTitle;
        private Label lblSubTitle;
        private Label lblPass1;
        private Label lblPass2;
        private TextBox txtPass1;
        private TextBox txtPass2;
        private Button btnXacNhan;
        private Button btnQuayLai;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            panelCard = new RoundedPanel();
            pictureLogo = new PictureBox();
            lblAppName = new Label();
            lblOnline = new Label(); // <--- Khởi tạo
            lblTitle = new Label();
            lblSubTitle = new Label();
            lblPass1 = new Label();
            lblPass2 = new Label();
            txtPass1 = new TextBox();
            txtPass2 = new TextBox();
            btnXacNhan = new Button();
            btnQuayLai = new Button();

            ((System.ComponentModel.ISupportInitialize)pictureLogo).BeginInit();
            SuspendLayout();

            // FORM
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1152, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đặt lại mật khẩu";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.Black;

            try
            {
                BackgroundImage = Properties.Resources.Bg;
                BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { }

            // CARD
            panelCard.CornerRadius = 26;
            panelCard.BackColor = Color.FromArgb(24, 24, 32);
            panelCard.Size = new Size(560, 440);
            panelCard.Location = new Point(
                (ClientSize.Width - panelCard.Width) / 2,
                (ClientSize.Height - panelCard.Height) / 2);
            panelCard.Padding = new Padding(32);

            // LOGO
            pictureLogo.Size = new Size(56, 56);
            pictureLogo.Location = new Point(32, 24);
            pictureLogo.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                pictureLogo.Image = Properties.Resources.icon_knight;
            }
            catch { }

            // APP NAME: KỲ VƯƠNG
            lblAppName.AutoSize = true;
            lblAppName.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblAppName.ForeColor = Color.White;
            lblAppName.Location = new Point(100, 20);
            lblAppName.Text = "KỲ VƯƠNG";

            // LABEL: ONLINE
            lblOnline.AutoSize = true;
            lblOnline.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblOnline.ForeColor = Color.DeepSkyBlue;
            lblOnline.Location = new Point(104, 52);
            lblOnline.Text = "ONLINE"; // <--- Giữ nguyên tiếng Anh theo yêu cầu
            // ----------------------------------------

            // TITLE: ĐẶT LẠI MẬT KHẨU
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(32, 104);
            lblTitle.Text = "ĐẶT LẠI MẬT KHẨU"; // Đã Việt hóa

            // SUBTITLE
            lblSubTitle.AutoSize = true;
            lblSubTitle.Font = new Font("Segoe UI", 11F);
            lblSubTitle.ForeColor = Color.FromArgb(190, 190, 200);
            lblSubTitle.Location = new Point(34, 146);
            lblSubTitle.Text = "Nhập mật khẩu mới cho tài khoản của bạn.";

            // PASS1 LABEL
            lblPass1.AutoSize = true;
            lblPass1.Font = new Font("Segoe UI", 11F);
            lblPass1.ForeColor = Color.White;
            lblPass1.Location = new Point(34, 192);
            lblPass1.Text = "Mật khẩu mới";

            // PASS1 TEXTBOX
            txtPass1.Font = new Font("Segoe UI", 12F);
            txtPass1.ForeColor = Color.White;
            txtPass1.BackColor = Color.FromArgb(32, 32, 40);
            txtPass1.BorderStyle = BorderStyle.FixedSingle;
            txtPass1.Location = new Point(34, 215);
            txtPass1.Size = new Size(480, 34);
            txtPass1.UseSystemPasswordChar = true;
            txtPass1.TabIndex = 0;

            // PASS2 LABEL
            lblPass2.AutoSize = true;
            lblPass2.Font = new Font("Segoe UI", 11F);
            lblPass2.ForeColor = Color.White;
            lblPass2.Location = new Point(34, 258);
            lblPass2.Text = "Xác nhận mật khẩu";

            // PASS2 TEXTBOX
            txtPass2.Font = new Font("Segoe UI", 12F);
            txtPass2.ForeColor = Color.White;
            txtPass2.BackColor = Color.FromArgb(32, 32, 40);
            txtPass2.BorderStyle = BorderStyle.FixedSingle;
            txtPass2.Location = new Point(34, 281);
            txtPass2.Size = new Size(480, 34);
            txtPass2.UseSystemPasswordChar = true;
            txtPass2.TabIndex = 1;

            // BUTTON XÁC NHẬN
            btnXacNhan.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnXacNhan.BackColor = Color.FromArgb(37, 99, 235);
            btnXacNhan.ForeColor = Color.White;
            btnXacNhan.FlatStyle = FlatStyle.Flat;
            btnXacNhan.FlatAppearance.BorderSize = 0;
            btnXacNhan.Location = new Point(34, 332);
            btnXacNhan.Size = new Size(220, 44);
            btnXacNhan.TabIndex = 2;
            btnXacNhan.Text = "Đặt lại mật khẩu";
            btnXacNhan.UseVisualStyleBackColor = false;
            btnXacNhan.Click += btnXacNhan_Click;

            // BUTTON QUAY LẠI
            btnQuayLai.Font = new Font("Segoe UI", 10F);
            btnQuayLai.ForeColor = Color.FromArgb(82, 145, 255);
            btnQuayLai.BackColor = Color.Transparent;
            btnQuayLai.FlatStyle = FlatStyle.Flat;
            btnQuayLai.FlatAppearance.BorderSize = 0;
            btnQuayLai.Location = new Point(34, 382);
            btnQuayLai.Size = new Size(120, 30);
            btnQuayLai.TabIndex = 3;
            btnQuayLai.Text = "Quay lại";
            btnQuayLai.UseVisualStyleBackColor = true;
            btnQuayLai.Click += btnQuayLai_Click;

            // ADD CONTROLS
            panelCard.Controls.Add(pictureLogo);
            panelCard.Controls.Add(lblAppName);
            panelCard.Controls.Add(lblOnline); // <--- Add Control
            panelCard.Controls.Add(lblTitle);
            panelCard.Controls.Add(lblSubTitle);
            panelCard.Controls.Add(lblPass1);
            panelCard.Controls.Add(txtPass1);
            panelCard.Controls.Add(lblPass2);
            panelCard.Controls.Add(txtPass2);
            panelCard.Controls.Add(btnXacNhan);
            panelCard.Controls.Add(btnQuayLai);

            Controls.Add(panelCard);

            ((System.ComponentModel.ISupportInitialize)pictureLogo).EndInit();
            ResumeLayout(false);
        }
    }
}