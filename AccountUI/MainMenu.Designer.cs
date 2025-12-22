using System.Drawing;
using System.Windows.Forms;

namespace AccountUI
{
    partial class MainMenu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private RoundedPanel panelCard;
        private Panel panelIcon;
        private RoundedPanel panelKnightBg;
        private PictureBox picKnight;
        private Label lblChess;
        private Label lblOnline;
        private Label lblTitle;

        private Button btnProfile;
        private Button btnFriend;
        private Button button1;
        private Button btnCancelMatch;
        private Button btnCreateRoom;
        private Button btnJoinRoom;
        private Button button4;

        private Panel pnlCreatedId;
        private TextBox txtCreatedRoomId;

        private Panel pnlJoinId;
        private TextBox txtRoomId;

        private Label labelRoom;
        private PictureBox PicLeaderboard;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            panelCard = new RoundedPanel();
            PicLeaderboard = new PictureBox();
            button4 = new Button();
            panelIcon = new Panel();
            panelKnightBg = new RoundedPanel();
            picKnight = new PictureBox();
            lblChess = new Label();
            lblOnline = new Label();
            lblTitle = new Label();
            btnProfile = new Button();
            btnFriend = new Button();
            button1 = new Button();
            btnCreateRoom = new Button();
            btnJoinRoom = new Button();
            pnlCreatedId = new Panel();
            txtCreatedRoomId = new TextBox();
            pnlJoinId = new Panel();
            txtRoomId = new TextBox();
            btnCancelMatch = new Button();
            labelRoom = new Label();
            panelCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PicLeaderboard).BeginInit();
            panelIcon.SuspendLayout();
            panelKnightBg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picKnight).BeginInit();
            pnlCreatedId.SuspendLayout();
            pnlJoinId.SuspendLayout();
            SuspendLayout();
            // 
            // panelCard
            // 
            panelCard.BackColor = Color.FromArgb(18, 25, 40);
            panelCard.Controls.Add(PicLeaderboard);
            panelCard.Controls.Add(button4);
            panelCard.Controls.Add(panelIcon);
            panelCard.Controls.Add(lblTitle);
            panelCard.Controls.Add(btnProfile);
            panelCard.Controls.Add(btnFriend);
            panelCard.Controls.Add(button1);
            panelCard.Controls.Add(btnCreateRoom);
            panelCard.Controls.Add(btnJoinRoom);
            panelCard.Controls.Add(pnlCreatedId);
            panelCard.Controls.Add(pnlJoinId);
            panelCard.Controls.Add(btnCancelMatch);
            panelCard.Controls.Add(labelRoom);
            panelCard.CornerRadius = 32;
            panelCard.Location = new Point(406, 435);
            panelCard.Margin = new Padding(4, 5, 4, 5);
            panelCard.Name = "panelCard";
            panelCard.Size = new Size(686, 1067);
            panelCard.TabIndex = 0;
            // 
            // PicLeaderboard
            // 
            PicLeaderboard.Image = (Image)resources.GetObject("PicLeaderboard.Image");
            PicLeaderboard.Location = new Point(554, 192);
            PicLeaderboard.Margin = new Padding(4, 5, 4, 5);
            PicLeaderboard.Name = "PicLeaderboard";
            PicLeaderboard.Size = new Size(46, 98);
            PicLeaderboard.TabIndex = 12;
            PicLeaderboard.TabStop = false;
            PicLeaderboard.Click += PicLeaderboard_Click;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button4.BackColor = Color.FromArgb(40, 46, 64);
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button4.ForeColor = Color.White;
            button4.Location = new Point(429, 998);
            button4.Margin = new Padding(4, 5, 4, 5);
            button4.Name = "button4";
            button4.Size = new Size(171, 63);
            button4.TabIndex = 11;
            button4.Text = "Đăng xuất";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // panelIcon
            // 
            panelIcon.BackColor = Color.Transparent;
            panelIcon.Controls.Add(panelKnightBg);
            panelIcon.Controls.Add(lblChess);
            panelIcon.Controls.Add(lblOnline);
            panelIcon.Location = new Point(107, 47);
            panelIcon.Margin = new Padding(4, 5, 4, 5);
            panelIcon.Name = "panelIcon";
            panelIcon.Size = new Size(471, 150);
            panelIcon.TabIndex = 0;
            // 
            // panelKnightBg
            // 
            panelKnightBg.BackColor = Color.FromArgb(40, 46, 64);
            panelKnightBg.Controls.Add(picKnight);
            panelKnightBg.CornerRadius = 14;
            panelKnightBg.Location = new Point(0, 25);
            panelKnightBg.Margin = new Padding(4, 5, 4, 5);
            panelKnightBg.Name = "panelKnightBg";
            panelKnightBg.Size = new Size(86, 100);
            panelKnightBg.TabIndex = 0;
            // 
            // picKnight
            // 
            picKnight.Dock = DockStyle.Fill;
            picKnight.Location = new Point(0, 0);
            picKnight.Margin = new Padding(4, 5, 4, 5);
            picKnight.Name = "picKnight";
            picKnight.Size = new Size(86, 100);
            picKnight.SizeMode = PictureBoxSizeMode.Zoom;
            picKnight.TabIndex = 0;
            picKnight.TabStop = false;
            // 
            // lblChess
            // 
            lblChess.AutoSize = true;
            lblChess.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblChess.ForeColor = Color.White;
            lblChess.Location = new Point(114, 30);
            lblChess.Margin = new Padding(4, 0, 4, 0);
            lblChess.Name = "lblChess";
            lblChess.Size = new Size(209, 48);
            lblChess.TabIndex = 1;
            lblChess.Text = "KỲ VƯƠNG";
            // 
            // lblOnline
            // 
            lblOnline.AutoSize = true;
            lblOnline.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblOnline.ForeColor = Color.DeepSkyBlue;
            lblOnline.Location = new Point(104, 52);
            lblOnline.Text = "ONLINE"; // <--- Đã sửa thành Tiếng Việt
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(235, 238, 245);
            lblTitle.Location = new Point(214, 192);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(257, 60);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "SẢNH CHỜ";
            // 
            // btnProfile
            // 
            btnProfile.BackColor = Color.FromArgb(50, 130, 255);
            btnProfile.FlatAppearance.BorderSize = 0;
            btnProfile.FlatStyle = FlatStyle.Flat;
            btnProfile.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnProfile.ForeColor = Color.White;
            btnProfile.Location = new Point(86, 292);
            btnProfile.Margin = new Padding(4, 5, 4, 5);
            btnProfile.Name = "btnProfile";
            btnProfile.Size = new Size(514, 80);
            btnProfile.TabIndex = 2;
            btnProfile.Text = "Hồ sơ người chơi";
            btnProfile.UseVisualStyleBackColor = false;
            btnProfile.Click += btnProfile_Click;
            // 
            // btnFriend
            // 
            btnFriend.BackColor = Color.FromArgb(35, 45, 65);
            btnFriend.FlatAppearance.BorderSize = 0;
            btnFriend.FlatStyle = FlatStyle.Flat;
            btnFriend.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnFriend.ForeColor = Color.White;
            btnFriend.Location = new Point(86, 388);
            btnFriend.Margin = new Padding(4, 5, 4, 5);
            btnFriend.Name = "btnFriend";
            btnFriend.Size = new Size(514, 80);
            btnFriend.TabIndex = 3;
            btnFriend.Text = "Bạn bè";
            btnFriend.UseVisualStyleBackColor = false;
            btnFriend.Click += btnFriend_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(35, 45, 65);
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button1.ForeColor = Color.White;
            button1.Location = new Point(86, 485);
            button1.Margin = new Padding(4, 5, 4, 5);
            button1.Name = "button1";
            button1.Size = new Size(514, 80);
            button1.TabIndex = 4;
            button1.Text = "Ghép Trận Ngẫu Nhiên";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.BackColor = Color.FromArgb(35, 45, 65);
            btnCreateRoom.FlatAppearance.BorderSize = 0;
            btnCreateRoom.FlatStyle = FlatStyle.Flat;
            btnCreateRoom.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCreateRoom.ForeColor = Color.White;
            btnCreateRoom.Location = new Point(86, 582);
            btnCreateRoom.Margin = new Padding(4, 5, 4, 5);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(514, 80);
            btnCreateRoom.TabIndex = 5;
            btnCreateRoom.Text = "Tạo Phòng";
            btnCreateRoom.UseVisualStyleBackColor = false;
            btnCreateRoom.Click += btnCreateRoom_Click;
            // 
            // btnJoinRoom
            // 
            btnJoinRoom.BackColor = Color.FromArgb(35, 45, 65);
            btnJoinRoom.FlatAppearance.BorderSize = 0;
            btnJoinRoom.FlatStyle = FlatStyle.Flat;
            btnJoinRoom.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnJoinRoom.ForeColor = Color.White;
            btnJoinRoom.Location = new Point(86, 678);
            btnJoinRoom.Margin = new Padding(4, 5, 4, 5);
            btnJoinRoom.Name = "btnJoinRoom";
            btnJoinRoom.Size = new Size(514, 80);
            btnJoinRoom.TabIndex = 6;
            btnJoinRoom.Text = "Tìm / Vào Phòng";
            btnJoinRoom.UseVisualStyleBackColor = false;
            btnJoinRoom.Click += btnJoinRoom_Click;
            // 
            // pnlCreatedId
            // 
            pnlCreatedId.BackColor = Color.FromArgb(35, 45, 65);
            pnlCreatedId.Controls.Add(txtCreatedRoomId);
            pnlCreatedId.Location = new Point(86, 783);
            pnlCreatedId.Margin = new Padding(4, 5, 4, 5);
            pnlCreatedId.Name = "pnlCreatedId";
            pnlCreatedId.Size = new Size(514, 87);
            pnlCreatedId.TabIndex = 7;
            pnlCreatedId.Visible = false;
            // 
            // txtCreatedRoomId
            // 
            txtCreatedRoomId.BackColor = Color.FromArgb(35, 45, 65);
            txtCreatedRoomId.BorderStyle = BorderStyle.None;
            txtCreatedRoomId.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            txtCreatedRoomId.ForeColor = Color.Cyan;
            txtCreatedRoomId.Location = new Point(14, 23);
            txtCreatedRoomId.Margin = new Padding(4, 5, 4, 5);
            txtCreatedRoomId.Name = "txtCreatedRoomId";
            txtCreatedRoomId.Size = new Size(486, 38);
            txtCreatedRoomId.TabIndex = 0;
            txtCreatedRoomId.TextAlign = HorizontalAlignment.Center;
            // 
            // pnlJoinId
            // 
            pnlJoinId.BackColor = Color.FromArgb(35, 45, 65);
            pnlJoinId.Controls.Add(txtRoomId);
            pnlJoinId.Location = new Point(86, 783);
            pnlJoinId.Margin = new Padding(4, 5, 4, 5);
            pnlJoinId.Name = "pnlJoinId";
            pnlJoinId.Size = new Size(514, 87);
            pnlJoinId.TabIndex = 8;
            pnlJoinId.Visible = false;
            // 
            // txtRoomId
            // 
            txtRoomId.BackColor = Color.FromArgb(35, 45, 65);
            txtRoomId.BorderStyle = BorderStyle.None;
            txtRoomId.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtRoomId.ForeColor = Color.White;
            txtRoomId.Location = new Point(14, 27);
            txtRoomId.Margin = new Padding(4, 5, 4, 5);
            txtRoomId.Name = "txtRoomId";
            txtRoomId.Size = new Size(486, 32);
            txtRoomId.TabIndex = 0;
            txtRoomId.TextAlign = HorizontalAlignment.Center;
            // 
            // btnCancelMatch
            // 
            btnCancelMatch.BackColor = Color.FromArgb(170, 60, 60);
            btnCancelMatch.FlatAppearance.BorderSize = 0;
            btnCancelMatch.FlatStyle = FlatStyle.Flat;
            btnCancelMatch.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancelMatch.ForeColor = Color.White;
            btnCancelMatch.Location = new Point(86, 887);
            btnCancelMatch.Margin = new Padding(4, 5, 4, 5);
            btnCancelMatch.Name = "btnCancelMatch";
            btnCancelMatch.Size = new Size(514, 70);
            btnCancelMatch.TabIndex = 9;
            btnCancelMatch.Text = "Hủy ghép";
            btnCancelMatch.UseVisualStyleBackColor = false;
            btnCancelMatch.Visible = false;
            btnCancelMatch.Click += btnCancelMatch_Click;
            // 
            // labelRoom
            // 
            labelRoom.BackColor = Color.Transparent;
            labelRoom.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            labelRoom.ForeColor = Color.FromArgb(245, 220, 90);
            labelRoom.Location = new Point(86, 958);
            labelRoom.Margin = new Padding(4, 0, 4, 0);
            labelRoom.Name = "labelRoom";
            labelRoom.Size = new Size(514, 40);
            labelRoom.TabIndex = 10;
            labelRoom.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Bg;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1714, 1050);
            Controls.Add(panelCard);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            Name = "MainMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kỳ Vương Online - Sảnh Chờ"; // Đã Việt Hóa
            panelCard.ResumeLayout(false);
            panelCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PicLeaderboard).EndInit();
            panelIcon.ResumeLayout(false);
            panelIcon.PerformLayout();
            panelKnightBg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picKnight).EndInit();
            pnlCreatedId.ResumeLayout(false);
            pnlCreatedId.PerformLayout();
            pnlJoinId.ResumeLayout(false);
            pnlJoinId.PerformLayout();
            ResumeLayout(false);
        }
    }
}