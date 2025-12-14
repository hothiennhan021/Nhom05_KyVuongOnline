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

        private void InitializeComponent()
        {
            panelCard = new RoundedPanel();
            panelIcon = new Panel();
            panelKnightBg = new RoundedPanel();
            picKnight = new PictureBox();
            lblChess = new Label();
            lblOnline = new Label();
            lblTitle = new Label();
            btnProfile = new Button();
            btnFriend = new Button();
            button1 = new Button();
            btnCancelMatch = new Button();
            btnCreateRoom = new Button();
            btnJoinRoom = new Button();
            button4 = new Button();

            pnlCreatedId = new Panel();
            txtCreatedRoomId = new TextBox();

            pnlJoinId = new Panel();
            txtRoomId = new TextBox();

            labelRoom = new Label();

            panelCard.SuspendLayout();
            panelIcon.SuspendLayout();
            panelKnightBg.SuspendLayout();
            pnlCreatedId.SuspendLayout();
            pnlJoinId.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(picKnight)).BeginInit();
            SuspendLayout();

            // ===== FORM =====
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Chess Online - Main Menu";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackgroundImage = Properties.Resources.Bg;
            BackgroundImageLayout = ImageLayout.Stretch;

            // ===== CARD =====
            panelCard.Size = new Size(480, 640);
            panelCard.CornerRadius = 32;
            panelCard.BackColor = Color.FromArgb(18, 25, 40);
            panelCard.Location = new Point((ClientSize.Width - panelCard.Width) / 2, (ClientSize.Height - panelCard.Height) / 2);

            // ===== LOGO =====
            panelIcon.Size = new Size(330, 90);
            panelIcon.BackColor = Color.Transparent;
            panelIcon.Location = new Point(75, 28);

            panelKnightBg.Size = new Size(60, 60);
            panelKnightBg.CornerRadius = 14;
            panelKnightBg.BackColor = Color.FromArgb(40, 46, 64);
            panelKnightBg.Location = new Point(0, 15);
            picKnight.Dock = DockStyle.Fill;
            picKnight.SizeMode = PictureBoxSizeMode.Zoom;
            panelKnightBg.Controls.Add(picKnight);

            lblChess.AutoSize = true;
            lblChess.Text = "CHESS";
            lblChess.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblChess.ForeColor = Color.White;
            lblChess.Location = new Point(80, 18);

            lblOnline.AutoSize = true;
            lblOnline.Text = "ONLINE";
            lblOnline.Font = new Font("Segoe UI", 9F);
            lblOnline.ForeColor = Color.FromArgb(190, 195, 205);
            lblOnline.Location = new Point(83, 50);

            panelIcon.Controls.Add(panelKnightBg);
            panelIcon.Controls.Add(lblChess);
            panelIcon.Controls.Add(lblOnline);

            // ===== TITLE (Y=115 - Sát lên một chút) =====
            lblTitle.AutoSize = true;
            lblTitle.Text = "Main Menu";
            lblTitle.ForeColor = Color.FromArgb(235, 238, 245);
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTitle.Location = new Point(150, 115);

            // ===== BUTTONS (Y Start=175, Pitch=58) =====
            Font btnFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            int btnX = 60;
            int btnW = 360;
            int btnH = 48;

            // 1. Profile (Y: 175 - Cách Title nhiều hơn)
            btnProfile.Text = "Hồ sơ người chơi";
            btnProfile.Size = new Size(btnW, btnH);
            btnProfile.Location = new Point(btnX, 175);
            btnProfile.Font = btnFont;
            btnProfile.BackColor = Color.FromArgb(50, 130, 255);
            btnProfile.ForeColor = Color.White;
            btnProfile.FlatStyle = FlatStyle.Flat;
            btnProfile.FlatAppearance.BorderSize = 0;
            btnProfile.Click += btnProfile_Click;

            // 2. Friend (Y: 233)
            btnFriend.Text = "Bạn bè";
            btnFriend.Size = new Size(btnW, btnH);
            btnFriend.Location = new Point(btnX, 233);
            btnFriend.Font = btnFont;
            btnFriend.BackColor = Color.FromArgb(35, 45, 65);
            btnFriend.ForeColor = Color.White;
            btnFriend.FlatStyle = FlatStyle.Flat;
            btnFriend.FlatAppearance.BorderSize = 0;
            btnFriend.Click += btnFriend_Click;

            // 3. Match (Y: 291)
            button1.Text = "Ghép Trận Ngẫu Nhiên";
            button1.Size = new Size(btnW, btnH);
            button1.Location = new Point(btnX, 291);
            button1.Font = btnFont;
            button1.BackColor = Color.FromArgb(35, 45, 65);
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Click += button1_Click;

            // 4. Create (Y: 349)
            btnCreateRoom.Text = "Tạo Phòng";
            btnCreateRoom.Size = new Size(btnW, btnH);
            btnCreateRoom.Location = new Point(btnX, 349);
            btnCreateRoom.Font = btnFont;
            btnCreateRoom.BackColor = Color.FromArgb(35, 45, 65);
            btnCreateRoom.ForeColor = Color.White;
            btnCreateRoom.FlatStyle = FlatStyle.Flat;
            btnCreateRoom.FlatAppearance.BorderSize = 0;
            btnCreateRoom.Click += btnCreateRoom_Click;

            // 5. Join (Y: 407)
            btnJoinRoom.Text = "Tìm / Vào Phòng";
            btnJoinRoom.Size = new Size(btnW, btnH);
            btnJoinRoom.Location = new Point(btnX, 407);
            btnJoinRoom.Font = btnFont;
            btnJoinRoom.BackColor = Color.FromArgb(35, 45, 65);
            btnJoinRoom.ForeColor = Color.White;
            btnJoinRoom.FlatStyle = FlatStyle.Flat;
            btnJoinRoom.FlatAppearance.BorderSize = 0;
            btnJoinRoom.Click += btnJoinRoom_Click;

            // ===== PANELS (Y: 470) =====
            int panelY = 470;

            // Created ID
            pnlCreatedId.Size = new Size(360, 52);
            pnlCreatedId.BackColor = Color.FromArgb(35, 45, 65);
            pnlCreatedId.Location = new Point(btnX, panelY);
            pnlCreatedId.Visible = false;

            txtCreatedRoomId.BorderStyle = BorderStyle.None;
            txtCreatedRoomId.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            txtCreatedRoomId.BackColor = pnlCreatedId.BackColor;
            // Màu CYAN đã set ở code C#, nhưng set ở đây để design view cũng thấy
            txtCreatedRoomId.ForeColor = Color.Cyan;
            txtCreatedRoomId.Width = 340;
            txtCreatedRoomId.Location = new Point(10, 14);
            txtCreatedRoomId.TextAlign = HorizontalAlignment.Center;
            pnlCreatedId.Controls.Add(txtCreatedRoomId);

            // Join ID
            pnlJoinId.Size = new Size(360, 52);
            pnlJoinId.BackColor = Color.FromArgb(35, 45, 65);
            pnlJoinId.Location = new Point(btnX, panelY);
            pnlJoinId.Visible = false;

            txtRoomId.BorderStyle = BorderStyle.None;
            txtRoomId.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtRoomId.BackColor = pnlJoinId.BackColor;
            txtRoomId.ForeColor = Color.White;
            txtRoomId.Width = 340;
            txtRoomId.Location = new Point(10, 16);
            txtRoomId.TextAlign = HorizontalAlignment.Center;
            pnlJoinId.Controls.Add(txtRoomId);

            // ===== CANCEL BUTTON (Y: 532) =====
            // Nằm dưới panel một chút
            btnCancelMatch.Text = "Hủy ghép";
            btnCancelMatch.Size = new Size(360, 42);
            btnCancelMatch.Location = new Point(btnX, 532);
            btnCancelMatch.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancelMatch.BackColor = Color.FromArgb(170, 60, 60);
            btnCancelMatch.ForeColor = Color.White;
            btnCancelMatch.FlatStyle = FlatStyle.Flat;
            btnCancelMatch.FlatAppearance.BorderSize = 0;
            btnCancelMatch.Visible = false;
            btnCancelMatch.Click += btnCancelMatch_Click;

            // Label Status
            labelRoom.BackColor = Color.Transparent;
            labelRoom.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            labelRoom.ForeColor = Color.FromArgb(245, 220, 90);
            labelRoom.Size = new Size(360, 24);
            labelRoom.Location = new Point(btnX, 575);
            labelRoom.TextAlign = ContentAlignment.MiddleCenter;

            // ===== LOGOUT (Y ~ 586) =====
            // Khoảng cách từ đáy nút Hủy (532 + 42 = 574) đến đây là 12px. An toàn.
            button4.Text = "Đăng xuất";
            button4.Size = new Size(120, 38);
            button4.Location = new Point(panelCard.Width - 120 - 18, panelCard.Height - 38 - 16);
            button4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button4.BackColor = Color.FromArgb(40, 46, 64);
            button4.ForeColor = Color.White;
            button4.FlatStyle = FlatStyle.Flat;
            button4.FlatAppearance.BorderSize = 0;
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button4.Click += button4_Click;

            // ===== ADD CONTROLS =====
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
            panelCard.Controls.Add(button4);

            Controls.Add(panelCard);

            panelCard.ResumeLayout(false);
            panelCard.PerformLayout();
            panelIcon.ResumeLayout(false);
            panelIcon.PerformLayout();
            panelKnightBg.ResumeLayout(false);
            pnlCreatedId.ResumeLayout(false);
            pnlCreatedId.PerformLayout();
            pnlJoinId.ResumeLayout(false);
            pnlJoinId.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(picKnight)).EndInit();
            ResumeLayout(false);
        }
    }
}