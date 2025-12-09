using System.Drawing;
using System.Windows.Forms;

namespace AccountUI
{
    partial class Signup
    {
        private System.ComponentModel.IContainer components = null;

        private RoundedPanel cardPanel;

        // Header
        private Panel panelHeader;
        private RoundedPanel panelKnightBg;
        private PictureBox logoPictureBox;
        private Label labelBrand;
        private Label labelBrandSub;
        private Label labelTitle;

        // Panels input
        private RoundedPanel panelUsername;
        private RoundedPanel panelEmail;
        private RoundedPanel panelOtp;
        private RoundedPanel panelFullName;
        private RoundedPanel panelBirthday;
        private RoundedPanel panelPassword;
        private RoundedPanel panelConfirmPassword;

        // Controls
        private TextBox textBox1;      // Username
        private TextBox txtEmail;
        private TextBox txtOtp;
        private TextBox txtFullName;
        private DateTimePicker dtpBirthday;
        private TextBox textBox3;      // Password
        private TextBox textBox4;      // Confirm password

        private Button btnSendOtp;
        private Button btnVerifyOtp;
        private Button btnSignup;
        private Button button2;

        private Button button_passwordshow;
        private Button button_passwordhide;
        private Button button_passwordshow2;
        private Button button_passwordhide2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            cardPanel = new RoundedPanel();
            panelHeader = new Panel();
            panelKnightBg = new RoundedPanel();
            logoPictureBox = new PictureBox();
            labelBrand = new Label();
            labelBrandSub = new Label();
            labelTitle = new Label();
            panelUsername = new RoundedPanel();
            textBox1 = new TextBox();
            panelEmail = new RoundedPanel();
            txtEmail = new TextBox();
            panelOtp = new RoundedPanel();
            txtOtp = new TextBox();
            btnSendOtp = new Button();
            btnVerifyOtp = new Button();
            panelFullName = new RoundedPanel();
            txtFullName = new TextBox();
            panelBirthday = new RoundedPanel();
            dtpBirthday = new DateTimePicker();
            panelPassword = new RoundedPanel();
            textBox3 = new TextBox();
            button_passwordshow = new Button();
            button_passwordhide = new Button();
            panelConfirmPassword = new RoundedPanel();
            textBox4 = new TextBox();
            button_passwordshow2 = new Button();
            button_passwordhide2 = new Button();
            btnSignup = new Button();
            button2 = new Button();
            cardPanel.SuspendLayout();
            panelHeader.SuspendLayout();
            panelKnightBg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
            panelUsername.SuspendLayout();
            panelEmail.SuspendLayout();
            panelOtp.SuspendLayout();
            panelFullName.SuspendLayout();
            panelBirthday.SuspendLayout();
            panelPassword.SuspendLayout();
            panelConfirmPassword.SuspendLayout();
            SuspendLayout();
            // 
            // cardPanel
            // 
            cardPanel.BackColor = Color.FromArgb(18, 25, 40);
            cardPanel.Controls.Add(panelHeader);
            cardPanel.Controls.Add(labelTitle);
            cardPanel.Controls.Add(panelUsername);
            cardPanel.Controls.Add(panelEmail);
            cardPanel.Controls.Add(panelOtp);
            cardPanel.Controls.Add(btnSendOtp);
            cardPanel.Controls.Add(btnVerifyOtp);
            cardPanel.Controls.Add(panelFullName);
            cardPanel.Controls.Add(panelBirthday);
            cardPanel.Controls.Add(panelPassword);
            cardPanel.Controls.Add(panelConfirmPassword);
            cardPanel.Controls.Add(btnSignup);
            cardPanel.Controls.Add(button2);
            cardPanel.CornerRadius = 32;
            cardPanel.Location = new Point(240, 60);
            cardPanel.Name = "cardPanel";
            cardPanel.Size = new Size(720, 600);
            cardPanel.TabIndex = 0;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.Transparent;
            panelHeader.Controls.Add(panelKnightBg);
            panelHeader.Controls.Add(labelBrand);
            panelHeader.Controls.Add(labelBrandSub);
            panelHeader.Location = new Point(195, 19);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(330, 100);
            panelHeader.TabIndex = 0;
            // 
            // panelKnightBg
            // 
            panelKnightBg.BackColor = Color.FromArgb(40, 46, 64);
            panelKnightBg.Controls.Add(logoPictureBox);
            panelKnightBg.CornerRadius = 18;
            panelKnightBg.Location = new Point(0, 14);
            panelKnightBg.Name = "panelKnightBg";
            panelKnightBg.Size = new Size(72, 72);
            panelKnightBg.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            logoPictureBox.Dock = DockStyle.Fill;
            logoPictureBox.Location = new Point(0, 0);
            logoPictureBox.Name = "logoPictureBox";
            logoPictureBox.Size = new Size(72, 72);
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoPictureBox.TabIndex = 0;
            logoPictureBox.TabStop = false;
            // 
            // labelBrand
            // 
            labelBrand.AutoSize = true;
            labelBrand.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            labelBrand.ForeColor = Color.White;
            labelBrand.Location = new Point(90, 18);
            labelBrand.Name = "labelBrand";
            labelBrand.Size = new Size(123, 46);
            labelBrand.TabIndex = 1;
            labelBrand.Text = "CHESS";
            // 
            // labelBrandSub
            // 
            labelBrandSub.AutoSize = true;
            labelBrandSub.Font = new Font("Segoe UI", 10F);
            labelBrandSub.ForeColor = Color.FromArgb(190, 195, 205);
            labelBrandSub.Location = new Point(94, 50);
            labelBrandSub.Name = "labelBrandSub";
            labelBrandSub.Size = new Size(71, 23);
            labelBrandSub.TabIndex = 2;
            labelBrandSub.Text = "ONLINE";
            // 
            // labelTitle
            // 
            labelTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            labelTitle.ForeColor = Color.FromArgb(235, 238, 245);
            labelTitle.Location = new Point(0, 142);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(720, 50);
            labelTitle.TabIndex = 1;
            labelTitle.Text = "Register";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelUsername
            // 
            panelUsername.BackColor = Color.FromArgb(35, 45, 65);
            panelUsername.Controls.Add(textBox1);
            panelUsername.CornerRadius = 16;
            panelUsername.Location = new Point(60, 195);
            panelUsername.Name = "panelUsername";
            panelUsername.Padding = new Padding(14, 10, 14, 10);
            panelUsername.Size = new Size(600, 48);
            panelUsername.TabIndex = 2;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(35, 45, 65);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Dock = DockStyle.Fill;
            textBox1.Font = new Font("Segoe UI", 11.5F);
            textBox1.ForeColor = Color.FromArgb(180, 182, 196);
            textBox1.Location = new Point(14, 10);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(572, 26);
            textBox1.TabIndex = 0;
            textBox1.Text = "Tên Đăng Nhập";
            textBox1.Enter += textBox1_Enter;
            textBox1.Leave += textBox1_Leave;
            // 
            // panelEmail
            // 
            panelEmail.BackColor = Color.FromArgb(35, 45, 65);
            panelEmail.Controls.Add(txtEmail);
            panelEmail.CornerRadius = 16;
            panelEmail.Location = new Point(60, 251);
            panelEmail.Name = "panelEmail";
            panelEmail.Padding = new Padding(14, 10, 14, 10);
            panelEmail.Size = new Size(600, 48);
            panelEmail.TabIndex = 3;
            // 
            // txtEmail
            // 
            txtEmail.BackColor = Color.FromArgb(35, 45, 65);
            txtEmail.BorderStyle = BorderStyle.None;
            txtEmail.Dock = DockStyle.Fill;
            txtEmail.Font = new Font("Segoe UI", 11.5F);
            txtEmail.ForeColor = Color.FromArgb(180, 182, 196);
            txtEmail.Location = new Point(14, 10);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(572, 26);
            txtEmail.TabIndex = 0;
            txtEmail.Text = "Email";
            txtEmail.Enter += textBox2_Enter;
            txtEmail.Leave += textBox2_Leave;
            // 
            // panelOtp
            // 
            panelOtp.BackColor = Color.FromArgb(35, 45, 65);
            panelOtp.Controls.Add(txtOtp);
            panelOtp.CornerRadius = 16;
            panelOtp.Location = new Point(60, 307);
            panelOtp.Name = "panelOtp";
            panelOtp.Padding = new Padding(14, 10, 14, 10);
            panelOtp.Size = new Size(285, 48);
            panelOtp.TabIndex = 4;
            // 
            // txtOtp
            // 
            txtOtp.BackColor = Color.FromArgb(35, 45, 65);
            txtOtp.BorderStyle = BorderStyle.None;
            txtOtp.Dock = DockStyle.Fill;
            txtOtp.Font = new Font("Segoe UI", 11.5F);
            txtOtp.ForeColor = Color.FromArgb(180, 182, 196);
            txtOtp.Location = new Point(14, 10);
            txtOtp.Name = "txtOtp";
            txtOtp.Size = new Size(257, 26);
            txtOtp.TabIndex = 0;
            txtOtp.Text = "Nhập mã OTP";
            txtOtp.Enter += txtOtp_Enter;
            txtOtp.Leave += txtOtp_Leave;
            // 
            // btnSendOtp
            // 
            btnSendOtp.BackColor = Color.FromArgb(50, 130, 255);
            btnSendOtp.FlatAppearance.BorderSize = 0;
            btnSendOtp.FlatStyle = FlatStyle.Flat;
            btnSendOtp.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSendOtp.ForeColor = Color.White;
            btnSendOtp.Location = new Point(402, 307);
            btnSendOtp.Name = "btnSendOtp";
            btnSendOtp.Size = new Size(100, 48);
            btnSendOtp.TabIndex = 5;
            btnSendOtp.Text = "Gửi OTP";
            btnSendOtp.UseVisualStyleBackColor = false;
            btnSendOtp.Click += btnSendOtp_Click;
            // 
            // btnVerifyOtp
            // 
            btnVerifyOtp.BackColor = Color.FromArgb(32, 34, 46);
            btnVerifyOtp.FlatAppearance.BorderSize = 0;
            btnVerifyOtp.FlatStyle = FlatStyle.Flat;
            btnVerifyOtp.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnVerifyOtp.ForeColor = Color.White;
            btnVerifyOtp.Location = new Point(528, 307);
            btnVerifyOtp.Name = "btnVerifyOtp";
            btnVerifyOtp.Size = new Size(100, 48);
            btnVerifyOtp.TabIndex = 6;
            btnVerifyOtp.Text = "Xác nhận";
            btnVerifyOtp.UseVisualStyleBackColor = false;
            btnVerifyOtp.Click += btnVerifyOtp_Click;
            // 
            // panelFullName
            // 
            panelFullName.BackColor = Color.FromArgb(35, 45, 65);
            panelFullName.Controls.Add(txtFullName);
            panelFullName.CornerRadius = 16;
            panelFullName.Location = new Point(60, 363);
            panelFullName.Name = "panelFullName";
            panelFullName.Padding = new Padding(14, 10, 14, 10);
            panelFullName.Size = new Size(285, 48);
            panelFullName.TabIndex = 7;
            // 
            // txtFullName
            // 
            txtFullName.BackColor = Color.FromArgb(35, 45, 65);
            txtFullName.BorderStyle = BorderStyle.None;
            txtFullName.Dock = DockStyle.Fill;
            txtFullName.Font = new Font("Segoe UI", 11.5F);
            txtFullName.ForeColor = Color.FromArgb(180, 182, 196);
            txtFullName.Location = new Point(14, 10);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(257, 26);
            txtFullName.TabIndex = 0;
            txtFullName.Text = "Họ và Tên";
            txtFullName.Enter += TxtFullName_Enter;
            txtFullName.Leave += TxtFullName_Leave;
            // 
            // panelBirthday
            // 
            panelBirthday.BackColor = Color.FromArgb(35, 45, 65);
            panelBirthday.Controls.Add(dtpBirthday);
            panelBirthday.CornerRadius = 16;
            panelBirthday.Location = new Point(375, 363);
            panelBirthday.Name = "panelBirthday";
            panelBirthday.Padding = new Padding(14, 6, 14, 6);
            panelBirthday.Size = new Size(285, 48);
            panelBirthday.TabIndex = 8;
            // 
            // dtpBirthday
            // 
            dtpBirthday.Dock = DockStyle.Fill;
            dtpBirthday.Font = new Font("Segoe UI", 11.5F);
            dtpBirthday.Format = DateTimePickerFormat.Short;
            dtpBirthday.Location = new Point(14, 6);
            dtpBirthday.Name = "dtpBirthday";
            dtpBirthday.Size = new Size(257, 33);
            dtpBirthday.TabIndex = 0;
            // 
            // panelPassword
            // 
            panelPassword.BackColor = Color.FromArgb(35, 45, 65);
            panelPassword.Controls.Add(textBox3);
            panelPassword.Controls.Add(button_passwordshow);
            panelPassword.Controls.Add(button_passwordhide);
            panelPassword.CornerRadius = 16;
            panelPassword.Location = new Point(60, 419);
            panelPassword.Name = "panelPassword";
            panelPassword.Size = new Size(285, 48);
            panelPassword.TabIndex = 9;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.FromArgb(35, 45, 65);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Font = new Font("Segoe UI", 11.5F);
            textBox3.ForeColor = Color.FromArgb(180, 182, 196);
            textBox3.Location = new Point(14, 13);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(285, 26);
            textBox3.TabIndex = 0;
            textBox3.Text = "Mật Khẩu";
            textBox3.Enter += textBox3_Enter;
            textBox3.Leave += textBox3_Leave;
            // 
            // button_passwordshow
            // 
            button_passwordshow.BackColor = Color.FromArgb(35, 45, 65);
            button_passwordshow.FlatAppearance.BorderSize = 0;
            button_passwordshow.FlatStyle = FlatStyle.Flat;
            button_passwordshow.Location = new Point(285, 48);
            button_passwordshow.Name = "button_passwordshow";
            button_passwordshow.Size = new Size(24, 24);
            button_passwordshow.TabIndex = 1;
            button_passwordshow.TabStop = false;
            button_passwordshow.UseVisualStyleBackColor = false;
            button_passwordshow.Click += button_passwordshow_Click;
            // 
            // button_passwordhide
            // 
            button_passwordhide.BackColor = Color.FromArgb(35, 45, 65);
            button_passwordhide.FlatAppearance.BorderSize = 0;
            button_passwordhide.FlatStyle = FlatStyle.Flat;
            button_passwordhide.Location = new Point(285, 48);
            button_passwordhide.Name = "button_passwordhide";
            button_passwordhide.Size = new Size(24, 24);
            button_passwordhide.TabIndex = 2;
            button_passwordhide.TabStop = false;
            button_passwordhide.UseVisualStyleBackColor = false;
            button_passwordhide.Visible = false;
            button_passwordhide.Click += button_passwordhide_Click;
            // 
            // panelConfirmPassword
            // 
            panelConfirmPassword.BackColor = Color.FromArgb(35, 45, 65);
            panelConfirmPassword.Controls.Add(textBox4);
            panelConfirmPassword.Controls.Add(button_passwordshow2);
            panelConfirmPassword.Controls.Add(button_passwordhide2);
            panelConfirmPassword.CornerRadius = 16;
            panelConfirmPassword.Location = new Point(375, 419);
            panelConfirmPassword.Name = "panelConfirmPassword";
            panelConfirmPassword.Size = new Size(285, 48);
            panelConfirmPassword.TabIndex = 10;
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.FromArgb(35, 45, 65);
            textBox4.BorderStyle = BorderStyle.None;
            textBox4.Font = new Font("Segoe UI", 11.5F);
            textBox4.ForeColor = Color.FromArgb(180, 182, 196);
            textBox4.Location = new Point(14, 13);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(285, 26);
            textBox4.TabIndex = 0;
            textBox4.Text = "Xác Nhận Mật Khẩu";
            textBox4.Enter += textBox4_Enter;
            textBox4.Leave += textBox4_Leave;
            // 
            // button_passwordshow2
            // 
            button_passwordshow2.BackColor = Color.FromArgb(35, 45, 65);
            button_passwordshow2.FlatAppearance.BorderSize = 0;
            button_passwordshow2.FlatStyle = FlatStyle.Flat;
            button_passwordshow2.Location = new Point(285, 48);
            button_passwordshow2.Name = "button_passwordshow2";
            button_passwordshow2.Size = new Size(24, 24);
            button_passwordshow2.TabIndex = 1;
            button_passwordshow2.TabStop = false;
            button_passwordshow2.UseVisualStyleBackColor = false;
            button_passwordshow2.Click += button_passwordshow2_Click;
            // 
            // button_passwordhide2
            // 
            button_passwordhide2.BackColor = Color.FromArgb(35, 45, 65);
            button_passwordhide2.FlatAppearance.BorderSize = 0;
            button_passwordhide2.FlatStyle = FlatStyle.Flat;
            button_passwordhide2.Location = new Point(285, 48);
            button_passwordhide2.Name = "button_passwordhide2";
            button_passwordhide2.Size = new Size(24, 24);
            button_passwordhide2.TabIndex = 2;
            button_passwordhide2.TabStop = false;
            button_passwordhide2.UseVisualStyleBackColor = false;
            button_passwordhide2.Visible = false;
            button_passwordhide2.Click += button_passwordhide2_Click;
            // 
            // btnSignup
            // 
            btnSignup.BackColor = Color.FromArgb(50, 130, 255);
            btnSignup.FlatAppearance.BorderSize = 0;
            btnSignup.FlatStyle = FlatStyle.Flat;
            btnSignup.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSignup.ForeColor = Color.White;
            btnSignup.Location = new Point(60, 475);
            btnSignup.Name = "btnSignup";
            btnSignup.Size = new Size(600, 48);
            btnSignup.TabIndex = 11;
            btnSignup.Text = "Đăng ký";
            btnSignup.UseVisualStyleBackColor = false;
            btnSignup.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(32, 34, 46);
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button2.ForeColor = Color.White;
            button2.Location = new Point(280, 533);
            button2.Name = "button2";
            button2.Size = new Size(160, 40);
            button2.TabIndex = 12;
            button2.Text = "Quay lại";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // Signup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 720);
            Controls.Add(cardPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Signup";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Chess Online - Sign up";
            Load += Signup_Load;
            Resize += Signup_Resize;
            cardPanel.ResumeLayout(false);
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelKnightBg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
            panelUsername.ResumeLayout(false);
            panelUsername.PerformLayout();
            panelEmail.ResumeLayout(false);
            panelEmail.PerformLayout();
            panelOtp.ResumeLayout(false);
            panelOtp.PerformLayout();
            panelFullName.ResumeLayout(false);
            panelFullName.PerformLayout();
            panelBirthday.ResumeLayout(false);
            panelPassword.ResumeLayout(false);
            panelPassword.PerformLayout();
            panelConfirmPassword.ResumeLayout(false);
            panelConfirmPassword.PerformLayout();
            ResumeLayout(false);
        }
    }
}
