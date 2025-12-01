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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            btnFriend = new Button();
            txtRoomId = new TextBox();
            btnJoinRoom = new Button();
            btnCreateRoom = new Button();
            labelRoom = new Label();
            btnProfile = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = Color.DarkSlateBlue;
            button1.Font = new Font("Times New Roman", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(341, 60);
            button1.Name = "button1";
            button1.Size = new Size(280, 55);
            button1.TabIndex = 0;
            button1.Text = "Ghép trận ngẫu nhiên";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(0, 0);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 8;
            button2.Visible = false;
            // 
            // button3
            // 
            button3.Location = new Point(0, 0);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 7;
            button3.Visible = false;
            // 
            // button4
            // 
            button4.BackColor = Color.DarkSlateBlue;
            button4.Font = new Font("Times New Roman", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.White;
            button4.Location = new Point(732, 440);
            button4.Name = "button4";
            button4.Size = new Size(180, 55);
            button4.TabIndex = 6;
            button4.Text = "Đăng Xuất";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // btnFriend
            // 
            btnFriend.BackColor = Color.DarkSlateBlue;
            btnFriend.Font = new Font("Times New Roman", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnFriend.ForeColor = Color.White;
            btnFriend.Location = new Point(50, 440);
            btnFriend.Name = "btnFriend";
            btnFriend.Size = new Size(180, 55);
            btnFriend.TabIndex = 5;
            btnFriend.Text = "Bạn bè";
            btnFriend.UseVisualStyleBackColor = false;
            btnFriend.Click += btnFriend_Click;
            // 
            // txtRoomId
            // 
            txtRoomId.Font = new Font("Times New Roman", 16F);
            txtRoomId.Location = new Point(341, 165);
            txtRoomId.Multiline = true;
            txtRoomId.Name = "txtRoomId";
            txtRoomId.PlaceholderText = "Nhập ID Phòng";
            txtRoomId.Size = new Size(280, 40);
            txtRoomId.TabIndex = 1;
            txtRoomId.TextAlign = HorizontalAlignment.Center;
            // 
            // btnJoinRoom
            // 
            btnJoinRoom.BackColor = Color.DarkSlateBlue;
            btnJoinRoom.Font = new Font("Times New Roman", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnJoinRoom.ForeColor = Color.White;
            btnJoinRoom.Location = new Point(341, 215);
            btnJoinRoom.Name = "btnJoinRoom";
            btnJoinRoom.Size = new Size(280, 55);
            btnJoinRoom.TabIndex = 2;
            btnJoinRoom.Text = "Vào Phòng";
            btnJoinRoom.UseVisualStyleBackColor = false;
            btnJoinRoom.Click += btnJoinRoom_Click;
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.BackColor = Color.DarkSlateBlue;
            btnCreateRoom.Font = new Font("Times New Roman", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnCreateRoom.ForeColor = Color.White;
            btnCreateRoom.Location = new Point(341, 320);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(280, 55);
            btnCreateRoom.TabIndex = 3;
            btnCreateRoom.Text = "Tạo Phòng";
            btnCreateRoom.UseVisualStyleBackColor = false;
            btnCreateRoom.Click += btnCreateRoom_Click;
            // 
            // labelRoom
            // 
            labelRoom.BackColor = Color.Transparent;
            labelRoom.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            labelRoom.ForeColor = Color.Yellow;
            labelRoom.Location = new Point(341, 380);
            labelRoom.Name = "labelRoom";
            labelRoom.Size = new Size(280, 30);
            labelRoom.TabIndex = 4;
            labelRoom.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnProfile
            // 
            btnProfile.BackColor = Color.DarkSlateBlue;
            btnProfile.Font = new Font("Times New Roman", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnProfile.ForeColor = Color.White;
            btnProfile.Location = new Point(0, 60);
            btnProfile.Name = "btnProfile";
            btnProfile.Size = new Size(280, 55);
            btnProfile.TabIndex = 9;
            btnProfile.Text = "Hồ Sơ Người Chơi";
            btnProfile.UseVisualStyleBackColor = false;
            btnProfile.Click += btnProfile_Click;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.BackGround;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(962, 521);
            Controls.Add(btnProfile);
            Controls.Add(labelRoom);
            Controls.Add(btnCreateRoom);
            Controls.Add(btnJoinRoom);
            Controls.Add(txtRoomId);
            Controls.Add(btnFriend);
            Controls.Add(button4);
            Controls.Add(button1);
            Controls.Add(button3);
            Controls.Add(button2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Game Menu";
            Load += MainMenu_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnFriend;
        private System.Windows.Forms.TextBox txtRoomId;
        private System.Windows.Forms.Button btnCreateRoom;
        private System.Windows.Forms.Button btnJoinRoom;
        private System.Windows.Forms.Label labelRoom;
        private Button btnProfile;
    }
}