using System.Windows.Forms;

namespace AccountUI
{
    partial class Login
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private RoundedPanel panelCard;

        // Logo
        private Panel panelIcon;
        private RoundedPanel panelKnightBg;
        private PictureBox picKnight;
        private Label lblChess;
        private Label lblOnline;

        private Label lblWelcome;

        private Panel pnlUser;
        private PictureBox picUser;
        private TextBox txtUser;

        private Panel pnlPass;
        private PictureBox picLock;
        private PictureBox picEye;
        private TextBox txtPass;

        private Button btnLogin;
        private LinkLabel linkForgot;
        private LinkLabel linkCreate;

        private void InitializeComponent()
        {
            this.panelCard = new AccountUI.RoundedPanel();

            this.panelIcon = new System.Windows.Forms.Panel();
            this.panelKnightBg = new AccountUI.RoundedPanel();
            this.picKnight = new System.Windows.Forms.PictureBox();
            this.lblChess = new System.Windows.Forms.Label();
            this.lblOnline = new System.Windows.Forms.Label();

            this.lblWelcome = new System.Windows.Forms.Label();

            this.pnlUser = new System.Windows.Forms.Panel();
            this.picUser = new System.Windows.Forms.PictureBox();
            this.txtUser = new System.Windows.Forms.TextBox();

            this.pnlPass = new System.Windows.Forms.Panel();
            this.picLock = new System.Windows.Forms.PictureBox();
            this.picEye = new System.Windows.Forms.PictureBox();
            this.txtPass = new System.Windows.Forms.TextBox();

            this.btnLogin = new System.Windows.Forms.Button();
            this.linkForgot = new System.Windows.Forms.LinkLabel();
            this.linkCreate = new System.Windows.Forms.LinkLabel();

            this.panelCard.SuspendLayout();
            this.panelIcon.SuspendLayout();
            this.panelKnightBg.SuspendLayout();
            this.pnlUser.SuspendLayout();
            this.pnlPass.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picKnight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEye)).BeginInit();
            this.SuspendLayout();

            // ===== FORM =====
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Chess Online - Login";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackgroundImage = Properties.Resources.Bg;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // ===== CARD =====
            this.panelCard.Size = new System.Drawing.Size(430, 540);
            this.panelCard.CornerRadius = 32;
            this.panelCard.BackColor = System.Drawing.Color.FromArgb(18, 25, 40);
            this.panelCard.Location = new System.Drawing.Point(
                (this.ClientSize.Width - 430) / 2,
                (this.ClientSize.Height - 540) / 2
            );

            // ===== LOGO CONTAINER (giống mẫu) =====
            this.panelIcon.Size = new System.Drawing.Size(330, 90);
            this.panelIcon.BackColor = System.Drawing.Color.Transparent;
            // 430 - 330 = 100 → lệch 50 mỗi bên
            this.panelIcon.Location = new System.Drawing.Point(50, 32);

            // --- ô vuông nền của knight ---
            this.panelKnightBg.Size = new System.Drawing.Size(60, 60);
            this.panelKnightBg.CornerRadius = 14;
            this.panelKnightBg.BackColor = System.Drawing.Color.FromArgb(40, 46, 64);
            this.panelKnightBg.Location = new System.Drawing.Point(0, 15);

            this.picKnight.Dock = DockStyle.Fill;
            this.picKnight.SizeMode = PictureBoxSizeMode.Zoom;

            this.panelKnightBg.Controls.Add(this.picKnight);

            // --- chữ CHESS ---
            this.lblChess.AutoSize = true;
            this.lblChess.Text = "CHESS";
            this.lblChess.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblChess.ForeColor = System.Drawing.Color.White;
            this.lblChess.Location = new System.Drawing.Point(80, 18);

            // --- chữ ONLINE ---
            this.lblOnline.AutoSize = true;
            this.lblOnline.Text = "ONLINE";
            this.lblOnline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblOnline.ForeColor = System.Drawing.Color.FromArgb(190, 195, 205);
            this.lblOnline.Location = new System.Drawing.Point(83, 50);

            this.panelIcon.Controls.Add(this.panelKnightBg);
            this.panelIcon.Controls.Add(this.lblChess);
            this.panelIcon.Controls.Add(this.lblOnline);

            // ===== WELCOME =====
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Text = "Welcome Back";
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(235, 238, 245);
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Location = new System.Drawing.Point(
                (this.panelCard.Width - 260) / 2,
                140
            );

            // ===== USER INPUT =====
            this.pnlUser.Size = new System.Drawing.Size(320, 52);
            this.pnlUser.BackColor = System.Drawing.Color.FromArgb(35, 45, 65);
            this.pnlUser.Location = new System.Drawing.Point(55, 215);

            this.picUser.Size = new System.Drawing.Size(28, 28);
            this.picUser.SizeMode = PictureBoxSizeMode.Zoom;
            this.picUser.Location = new System.Drawing.Point(
                14,
                (this.pnlUser.Height - this.picUser.Height) / 2
            );

            this.txtUser.BorderStyle = BorderStyle.None;
            this.txtUser.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.txtUser.BackColor = this.pnlUser.BackColor;
            this.txtUser.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtUser.Width = 250;
            this.txtUser.Location = new System.Drawing.Point(
                56,
                (this.pnlUser.Height - this.txtUser.Height) / 2 - 1
            );

            this.pnlUser.Controls.Add(this.picUser);
            this.pnlUser.Controls.Add(this.txtUser);

            // ===== PASSWORD INPUT =====
            this.pnlPass.Size = new System.Drawing.Size(320, 52);
            this.pnlPass.BackColor = System.Drawing.Color.FromArgb(35, 45, 65);
            this.pnlPass.Location = new System.Drawing.Point(55, 280);

            this.picLock.Size = new System.Drawing.Size(28, 28);
            this.picLock.SizeMode = PictureBoxSizeMode.Zoom;
            this.picLock.Location = new System.Drawing.Point(
                14,
                (this.pnlPass.Height - this.picLock.Height) / 2
            );

            this.txtPass.BorderStyle = BorderStyle.None;
            this.txtPass.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.txtPass.BackColor = this.pnlPass.BackColor;
            this.txtPass.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtPass.Width = 220;
            this.txtPass.Location = new System.Drawing.Point(
                56,
                (this.pnlPass.Height - this.txtPass.Height) / 2 - 1
            );

            this.picEye.Size = new System.Drawing.Size(28, 28);
            this.picEye.SizeMode = PictureBoxSizeMode.Zoom;
            this.picEye.Cursor = Cursors.Hand;
            this.picEye.Location = new System.Drawing.Point(
                this.pnlPass.Width - this.picEye.Width - 14,
                (this.pnlPass.Height - this.picEye.Height) / 2
            );

            this.pnlPass.Controls.Add(this.picLock);
            this.pnlPass.Controls.Add(this.txtPass);
            this.pnlPass.Controls.Add(this.picEye);

            // ===== LOGIN BUTTON =====
            this.btnLogin.Text = "LOGIN";
            this.btnLogin.Size = new System.Drawing.Size(320, 48);
            this.btnLogin.Location = new System.Drawing.Point(55, 350);
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(50, 130, 255);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;

            // ===== LINKS =====
            var linkNormal = System.Drawing.Color.FromArgb(180, 185, 195);
            var linkActive = System.Drawing.Color.FromArgb(230, 235, 245);

            this.linkForgot.AutoSize = true;
            this.linkForgot.Text = "Forgot password?";
            this.linkForgot.LinkColor = linkNormal;
            this.linkForgot.ActiveLinkColor = linkActive;
            this.linkForgot.Location = new System.Drawing.Point(165, 420);
            this.linkForgot.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkForgot_LinkClicked);

            this.linkCreate.AutoSize = true;
            this.linkCreate.Text = "Create account";
            this.linkCreate.LinkColor = System.Drawing.Color.FromArgb(100, 150, 255);
            this.linkCreate.ActiveLinkColor = linkActive;
            this.linkCreate.Location = new System.Drawing.Point(173, 450);
            this.linkCreate.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkCreate_LinkClicked);

            // ===== ADD TO CARD =====
            this.panelCard.Controls.Add(this.panelIcon);
            this.panelCard.Controls.Add(this.lblWelcome);
            this.panelCard.Controls.Add(this.pnlUser);
            this.panelCard.Controls.Add(this.pnlPass);
            this.panelCard.Controls.Add(this.btnLogin);
            this.panelCard.Controls.Add(this.linkForgot);
            this.panelCard.Controls.Add(this.linkCreate);

            // ===== ADD TO FORM =====
            this.Controls.Add(this.panelCard);

            this.panelCard.ResumeLayout(false);
            this.panelCard.PerformLayout();
            this.panelIcon.ResumeLayout(false);
            this.panelIcon.PerformLayout();
            this.panelKnightBg.ResumeLayout(false);
            this.pnlUser.ResumeLayout(false);
            this.pnlUser.PerformLayout();
            this.pnlPass.ResumeLayout(false);
            this.pnlPass.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
