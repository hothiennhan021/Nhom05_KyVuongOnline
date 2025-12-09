using System.Drawing;
using System.Windows.Forms;

namespace AccountUI
{
    partial class Recovery
    {
        private System.ComponentModel.IContainer components = null;

        private RoundedPanel panelCard;
        private PictureBox pictureLogo;
        private Label lblAppName;
        private Label lblTitle;
        private Label lblSubTitle;
        private Label lblEmail;
        private Label lblOtp;
        private TextBox txtEmail;
        private TextBox txtOTP;
        private Button btnGui;
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
            lblTitle = new Label();
            lblSubTitle = new Label();
            lblEmail = new Label();
            lblOtp = new Label();
            txtEmail = new TextBox();
            txtOTP = new TextBox();
            btnGui = new Button();
            btnXacNhan = new Button();
            btnQuayLai = new Button();

            ((System.ComponentModel.ISupportInitialize)pictureLogo).BeginInit();
            SuspendLayout();

            // ==== FORM ====
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1152, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quên mật khẩu";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.Black;

            // nền dùng resource Bg (giống login)
            try
            {
                BackgroundImage = Properties.Resources.Bg;
                BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { }

            // ==== CARD BO TRÒN ====
            panelCard.CornerRadius = 26;
            panelCard.BackColor = Color.FromArgb(24, 24, 32);
            panelCard.Size = new Size(560, 460);
            panelCard.Location = new Point(
                (ClientSize.Width - panelCard.Width) / 2,
                (ClientSize.Height - panelCard.Height) / 2);
            panelCard.Padding = new Padding(32);

            // ==== LOGO ====
            pictureLogo.Size = new Size(56, 56);
            pictureLogo.Location = new Point(32, 24);
            pictureLogo.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                pictureLogo.Image = Properties.Resources.icon_knight;
            }
            catch { }

            // ==== CHESS ONLINE ====
            lblAppName.AutoSize = true;
            lblAppName.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblAppName.ForeColor = Color.White;
            lblAppName.Location = new Point(104, 36);
            lblAppName.Text = "CHESS ONLINE";

            // ==== TITLE ====
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(32, 104);
            lblTitle.Text = "Forgot Password";

            // ==== SUBTITLE ====
            lblSubTitle.AutoSize = true;
            lblSubTitle.Font = new Font("Segoe UI", 11F);
            lblSubTitle.ForeColor = Color.FromArgb(190, 190, 200);
            lblSubTitle.Location = new Point(34, 146);
            lblSubTitle.Text = "Nhập email để nhận mã khôi phục mật khẩu.";

            // ==== EMAIL LABEL ====
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 11F);
            lblEmail.ForeColor = Color.White;
            lblEmail.Location = new Point(34, 192);
            lblEmail.Text = "Email";

            // ==== EMAIL TEXTBOX ====
            txtEmail.Font = new Font("Segoe UI", 12F);
            txtEmail.ForeColor = Color.White;
            txtEmail.BackColor = Color.FromArgb(32, 32, 40);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.Location = new Point(34, 215);
            txtEmail.Size = new Size(480, 34);
            txtEmail.TabIndex = 0;

            // ==== OTP LABEL ====
            lblOtp.AutoSize = true;
            lblOtp.Font = new Font("Segoe UI", 11F);
            lblOtp.ForeColor = Color.White;
            lblOtp.Location = new Point(34, 258);
            lblOtp.Text = "Mã xác nhận (OTP)";

            // ==== OTP TEXTBOX ====
            txtOTP.Font = new Font("Segoe UI", 12F);
            txtOTP.ForeColor = Color.White;
            txtOTP.BackColor = Color.FromArgb(32, 32, 40);
            txtOTP.BorderStyle = BorderStyle.FixedSingle;
            txtOTP.Location = new Point(34, 281);
            txtOTP.Size = new Size(480, 34);
            txtOTP.TabIndex = 1;

            // ==== BUTTON GỬI MÃ ====
            btnGui.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGui.BackColor = Color.FromArgb(37, 99, 235);
            btnGui.ForeColor = Color.White;
            btnGui.FlatStyle = FlatStyle.Flat;
            btnGui.FlatAppearance.BorderSize = 0;
            btnGui.Location = new Point(34, 332);
            btnGui.Size = new Size(220, 44);
            btnGui.TabIndex = 2;
            btnGui.Text = "Gửi mã";
            btnGui.UseVisualStyleBackColor = false;
            btnGui.Click += btnGui_Click;

            // ==== BUTTON XÁC NHẬN ====
            btnXacNhan.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnXacNhan.BackColor = Color.FromArgb(37, 99, 235);
            btnXacNhan.ForeColor = Color.White;
            btnXacNhan.FlatStyle = FlatStyle.Flat;
            btnXacNhan.FlatAppearance.BorderSize = 0;
            btnXacNhan.Location = new Point(294, 332);
            btnXacNhan.Size = new Size(220, 44);
            btnXacNhan.TabIndex = 3;
            btnXacNhan.Text = "Xác nhận";
            btnXacNhan.UseVisualStyleBackColor = false;
            btnXacNhan.Click += btnXacNhan_Click;

            // ==== BUTTON QUAY LẠI ====
            btnQuayLai.Font = new Font("Segoe UI", 10F);
            btnQuayLai.ForeColor = Color.FromArgb(82, 145, 255);
            btnQuayLai.BackColor = Color.Transparent;
            btnQuayLai.FlatStyle = FlatStyle.Flat;
            btnQuayLai.FlatAppearance.BorderSize = 0;
            btnQuayLai.Location = new Point(34, 386);
            btnQuayLai.Size = new Size(120, 30);
            btnQuayLai.TabIndex = 4;
            btnQuayLai.Text = "Quay lại";
            btnQuayLai.UseVisualStyleBackColor = true;
            btnQuayLai.Click += btnQuayLai_Click;

            // ADD TO CARD
            panelCard.Controls.Add(pictureLogo);
            panelCard.Controls.Add(lblAppName);
            panelCard.Controls.Add(lblTitle);
            panelCard.Controls.Add(lblSubTitle);
            panelCard.Controls.Add(lblEmail);
            panelCard.Controls.Add(txtEmail);
            panelCard.Controls.Add(lblOtp);
            panelCard.Controls.Add(txtOTP);
            panelCard.Controls.Add(btnGui);
            panelCard.Controls.Add(btnXacNhan);
            panelCard.Controls.Add(btnQuayLai);

            // ADD TO FORM
            Controls.Add(panelCard);

            ((System.ComponentModel.ISupportInitialize)pictureLogo).EndInit();
            ResumeLayout(false);
        }
    }
}
