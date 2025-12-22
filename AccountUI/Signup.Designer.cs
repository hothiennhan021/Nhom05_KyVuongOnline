using System.Drawing;
using System.Windows.Forms;

namespace AccountUI
{
    partial class Signup
    {
        private System.ComponentModel.IContainer components = null;

        private RoundedPanel cardPanel;

        private Panel panelHeader;
        private RoundedPanel panelKnightBg;
        private PictureBox logoPictureBox;
        private Label labelBrand;
        private Label labelBrandSub;
        private Label labelTitle;

        private RoundedPanel panelUsername;
        private RoundedPanel panelEmail;
        private RoundedPanel panelOtp;
        private RoundedPanel panelFullName;
        private RoundedPanel panelBirthday;
        private RoundedPanel panelPassword;
        private RoundedPanel panelConfirmPassword;

        private TextBox textBox1;
        private TextBox txtEmail;
        private TextBox txtOtp;
        private TextBox txtFullName;
        private DateTimePicker dtpBirthday;
        private TextBox textBox3;
        private TextBox textBox4;

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
            cardPanel.Location = new Point(300, 75);
            cardPanel.Margin = new Padding(4);
            cardPanel.Name = "cardPanel";
            cardPanel.Size = new Size(900, 750);
            cardPanel.TabIndex = 0;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.Transparent;
            panelHeader.Controls.Add(panelKnightBg);
            panelHeader.Controls.Add(labelBrand);
            panelHeader.Controls.Add(labelBrandSub);
            panelHeader.Location = new Point(244, 24);
            panelHeader.Margin = new Padding(4);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(412, 125);
            panelHeader.TabIndex = 0;
            // 
            // panelKnightBg
            // 
            panelKnightBg.BackColor = Color.FromArgb(40, 46, 64);
            panelKnightBg.Controls.Add(logoPictureBox);
            panelKnightBg.CornerRadius = 18;
            panelKnightBg.Location = new Point(0, 18);
            panelKnightBg.Margin = new Padding(4);
            panelKnightBg.Name = "panelKnightBg";
            panelKnightBg.Size = new Size(90, 90);
            panelKnightBg.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            logoPictureBox.Dock = DockStyle.Fill;
            logoPictureBox.Location = new Point(0, 0);
            logoPictureBox.Margin = new Padding(4);
            logoPictureBox.Name = "logoPictureBox";
            logoPictureBox.Size = new Size(90, 90);
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoPictureBox.TabIndex = 0;
            logoPictureBox.TabStop = false;
            // 
            // labelBrand
            // 
            labelBrand.AutoSize = true;
            labelBrand.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            labelBrand.ForeColor = Color.White;
            labelBrand.Location = new Point(112, 22);
            labelBrand.Margin = new Padding(4, 0, 4, 0);
            labelBrand.Name = "labelBrand";
            labelBrand.Size = new Size(233, 54);
            labelBrand.TabIndex = 1;
            labelBrand.Text = "KỲ VƯƠNG";
            // 
            // labelBrandSub
            // 
            labelBrandSub.AutoSize = true;
            labelBrandSub.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelBrandSub.ForeColor = Color.DeepSkyBlue;
            labelBrandSub.Location = new Point(118, 62);
            labelBrandSub.Margin = new Padding(4, 0, 4, 0);
            labelBrandSub.Name = "labelBrandSub";
            labelBrandSub.Size = new Size(103, 32);
            labelBrandSub.TabIndex = 2;
            labelBrandSub.Text = "ONLINE";
            // 
            // labelTitle
            // 
            labelTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            labelTitle.ForeColor = Color.FromArgb(235, 238, 245);
            labelTitle.Location = new Point(0, 178);
            labelTitle.Margin = new Padding(4, 0, 4, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(900, 62);
            labelTitle.TabIndex = 1;
            labelTitle.Text = "ĐĂNG KÝ TÀI KHOẢN";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelUsername
            // 
            panelUsername.BackColor = Color.FromArgb(35, 45, 65);
            panelUsername.Controls.Add(textBox1);
            panelUsername.CornerRadius = 16;
            panelUsername.Location = new Point(75, 244);
            panelUsername.Margin = new Padding(4);
            panelUsername.Name = "panelUsername";
            panelUsername.Padding = new Padding(18, 12, 18, 12);
            panelUsername.Size = new Size(750, 60);
            panelUsername.TabIndex = 2;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(35, 45, 65);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Dock = DockStyle.Fill;
            textBox1.Font = new Font("Segoe UI", 11.5F);
            textBox1.ForeColor = Color.FromArgb(180, 182, 196);
            textBox1.Location = new Point(18, 12);
            textBox1.Margin = new Padding(4);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(714, 31);
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
            panelEmail.Location = new Point(75, 314);
            panelEmail.Margin = new Padding(4);
            panelEmail.Name = "panelEmail";
            panelEmail.Padding = new Padding(18, 12, 18, 12);
            panelEmail.Size = new Size(750, 60);
            panelEmail.TabIndex = 3;
            // 
            // txtEmail
            // 
            txtEmail.BackColor = Color.FromArgb(35, 45, 65);
            txtEmail.BorderStyle = BorderStyle.None;
            txtEmail.Dock = DockStyle.Fill;
            txtEmail.Font = new Font("Segoe UI", 11.5F);
            txtEmail.ForeColor = Color.FromArgb(180, 182, 196);
            txtEmail.Location = new Point(18, 12);
            txtEmail.Margin = new Padding(4);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(714, 31);
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
            panelOtp.Location = new Point(75, 384);
            panelOtp.Margin = new Padding(4);
            panelOtp.Name = "panelOtp";
            panelOtp.Padding = new Padding(18, 12, 18, 12);
            panelOtp.Size = new Size(356, 60);
            panelOtp.TabIndex = 4;
            // 
            // txtOtp
            // 
            txtOtp.BackColor = Color.FromArgb(35, 45, 65);
            txtOtp.BorderStyle = BorderStyle.None;
            txtOtp.Dock = DockStyle.Fill;
            txtOtp.Font = new Font("Segoe UI", 11.5F);
            txtOtp.ForeColor = Color.FromArgb(180, 182, 196);
            txtOtp.Location = new Point(18, 12);
            txtOtp.Margin = new Padding(4);
            txtOtp.Name = "txtOtp";
            txtOtp.Size = new Size(320, 31);
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
            btnSendOtp.Location = new Point(502, 384);
            btnSendOtp.Margin = new Padding(4);
            btnSendOtp.Name = "btnSendOtp";
            btnSendOtp.Size = new Size(125, 60);
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
            btnVerifyOtp.Location = new Point(660, 384);
            btnVerifyOtp.Margin = new Padding(4);
            btnVerifyOtp.Name = "btnVerifyOtp";
            btnVerifyOtp.Size = new Size(125, 60);
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
            panelFullName.Location = new Point(75, 454);
            panelFullName.Margin = new Padding(4);
            panelFullName.Name = "panelFullName";
            panelFullName.Padding = new Padding(18, 12, 18, 12);
            panelFullName.Size = new Size(356, 60);
            panelFullName.TabIndex = 7;
            // 
            // txtFullName
            // 
            txtFullName.BackColor = Color.FromArgb(35, 45, 65);
            txtFullName.BorderStyle = BorderStyle.None;
            txtFullName.Dock = DockStyle.Fill;
            txtFullName.Font = new Font("Segoe UI", 11.5F);
            txtFullName.ForeColor = Color.FromArgb(180, 182, 196);
            txtFullName.Location = new Point(18, 12);
            txtFullName.Margin = new Padding(4);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(320, 31);
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
            panelBirthday.Location = new Point(469, 454);
            panelBirthday.Margin = new Padding(4);
            panelBirthday.Name = "panelBirthday";
            panelBirthday.Padding = new Padding(18, 8, 18, 8);
            panelBirthday.Size = new Size(356, 60);
            panelBirthday.TabIndex = 8;
            // 
            // dtpBirthday
            // 
            dtpBirthday.Dock = DockStyle.Fill;
            dtpBirthday.Font = new Font("Segoe UI", 11.5F);
            dtpBirthday.Format = DateTimePickerFormat.Short;
            dtpBirthday.Location = new Point(18, 8);
            dtpBirthday.Margin = new Padding(4);
            dtpBirthday.Name = "dtpBirthday";
            dtpBirthday.Size = new Size(320, 38);
            dtpBirthday.TabIndex = 0;
            // 
            // panelPassword
            // 
            panelPassword.BackColor = Color.FromArgb(35, 45, 65);
            panelPassword.Controls.Add(textBox3);
            panelPassword.Controls.Add(button_passwordshow);
            panelPassword.Controls.Add(button_passwordhide);
            panelPassword.CornerRadius = 16;
            panelPassword.Location = new Point(75, 524);
            panelPassword.Margin = new Padding(4);
            panelPassword.Name = "panelPassword";
            panelPassword.Size = new Size(356, 60);
            panelPassword.TabIndex = 9;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.FromArgb(35, 45, 65);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Font = new Font("Segoe UI", 11.5F);
            textBox3.ForeColor = Color.FromArgb(180, 182, 196);
            textBox3.Location = new Point(18, 16);
            textBox3.Margin = new Padding(4);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(275, 31);
            textBox3.TabIndex = 0;
            textBox3.Text = "Mật Khẩu";
            textBox3.Enter += textBox3_Enter;
            textBox3.Leave += textBox3_Leave;
            // 
            // button_passwordshow
            // 
            button_passwordshow.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_passwordshow.BackColor = Color.FromArgb(35, 45, 65);
            button_passwordshow.Cursor = Cursors.Hand;
            button_passwordshow.FlatAppearance.BorderSize = 0;
            button_passwordshow.FlatStyle = FlatStyle.Flat;
            button_passwordshow.Location = new Point(309, 12);
            button_passwordshow.Margin = new Padding(4);
            button_passwordshow.Name = "button_passwordshow";
            button_passwordshow.Size = new Size(35, 35);
            button_passwordshow.TabIndex = 1;
            button_passwordshow.TabStop = false;
            button_passwordshow.UseVisualStyleBackColor = false;
            button_passwordshow.Click += button_passwordshow_Click;
            // 
            // button_passwordhide
            // 
            button_passwordhide.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_passwordhide.BackColor = Color.FromArgb(35, 45, 65);
            button_passwordhide.Cursor = Cursors.Hand;
            button_passwordhide.FlatAppearance.BorderSize = 0;
            button_passwordhide.FlatStyle = FlatStyle.Flat;
            button_passwordhide.Location = new Point(309, 12);
            button_passwordhide.Margin = new Padding(4);
            button_passwordhide.Name = "button_passwordhide";
            button_passwordhide.Size = new Size(35, 35);
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
            panelConfirmPassword.Location = new Point(469, 524);
            panelConfirmPassword.Margin = new Padding(4);
            panelConfirmPassword.Name = "panelConfirmPassword";
            panelConfirmPassword.Size = new Size(356, 60);
            panelConfirmPassword.TabIndex = 10;
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.FromArgb(35, 45, 65);
            textBox4.BorderStyle = BorderStyle.None;
            textBox4.Font = new Font("Segoe UI", 11.5F);
            textBox4.ForeColor = Color.FromArgb(180, 182, 196);
            textBox4.Location = new Point(18, 16);
            textBox4.Margin = new Padding(4);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(275, 31);
            textBox4.TabIndex = 0;
            textBox4.Text = "Xác Nhận Mật Khẩu";
            textBox4.Enter += textBox4_Enter;
            textBox4.Leave += textBox4_Leave;
            // 
            // button_passwordshow2
            // 
            button_passwordshow2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_passwordshow2.BackColor = Color.FromArgb(35, 45, 65);
            button_passwordshow2.Cursor = Cursors.Hand;
            button_passwordshow2.FlatAppearance.BorderSize = 0;
            button_passwordshow2.FlatStyle = FlatStyle.Flat;
            button_passwordshow2.Location = new Point(309, 12);
            button_passwordshow2.Margin = new Padding(4);
            button_passwordshow2.Name = "button_passwordshow2";
            button_passwordshow2.Size = new Size(35, 35);
            button_passwordshow2.TabIndex = 1;
            button_passwordshow2.TabStop = false;
            button_passwordshow2.UseVisualStyleBackColor = false;
            button_passwordshow2.Click += button_passwordshow2_Click;
            // 
            // button_passwordhide2
            // 
            button_passwordhide2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_passwordhide2.BackColor = Color.FromArgb(35, 45, 65);
            button_passwordhide2.Cursor = Cursors.Hand;
            button_passwordhide2.FlatAppearance.BorderSize = 0;
            button_passwordhide2.FlatStyle = FlatStyle.Flat;
            button_passwordhide2.Location = new Point(309, 12);
            button_passwordhide2.Margin = new Padding(4);
            button_passwordhide2.Name = "button_passwordhide2";
            button_passwordhide2.Size = new Size(35, 35);
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
            btnSignup.Location = new Point(75, 594);
            btnSignup.Margin = new Padding(4);
            btnSignup.Name = "btnSignup";
            btnSignup.Size = new Size(750, 60);
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
            button2.Location = new Point(75, 662);
            button2.Margin = new Padding(4);
            button2.Name = "button2";
            button2.Size = new Size(200, 50);
            button2.TabIndex = 12;
            button2.Text = "Quay lại";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // Signup
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Bg;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1500, 900);
            Controls.Add(cardPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "Signup";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kỳ Vương Online - Đăng Ký";
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