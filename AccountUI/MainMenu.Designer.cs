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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnFriend = new System.Windows.Forms.Button();
            this.txtRoomId = new System.Windows.Forms.TextBox();
            this.btnJoinRoom = new System.Windows.Forms.Button();
            this.btnCreateRoom = new System.Windows.Forms.Button();
            this.labelRoom = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // =========================================================================
            // CẤU HÌNH THÔNG SỐ GIAO DIỆN (LAYOUT SYSTEM)
            // =========================================================================
            // Kích thước Form: 962 x 521
            // Chiều rộng nút chính: 280 (To hơn chút cho cân đối)
            // Chiều cao nút: 55
            // Trục giữa (Center X): (962 - 280) / 2 = 341
            // =========================================================================

            int centerX = 341;
            int btnWidth = 280;
            int btnHeight = 55;

            // Style chung (Giữ phong cách cũ)
            System.Drawing.Font mainFont = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            System.Drawing.Color btnColor = System.Drawing.Color.DarkSlateBlue;
            System.Drawing.Color txtColor = System.Drawing.Color.White;

            // 
            // 1. Button: Ghép trận ngẫu nhiên (Vị trí Y: 60)
            // 
            this.button1.AutoSize = false;
            this.button1.BackColor = btnColor;
            this.button1.Font = mainFont;
            this.button1.ForeColor = txtColor;
            this.button1.Location = new System.Drawing.Point(centerX, 60);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(btnWidth, btnHeight);
            this.button1.TabIndex = 0;
            this.button1.Text = "Ghép trận ngẫu nhiên";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);

            // 
            // --- CỤM VÀO PHÒNG (Cách nút trên 50px) ---
            // 

            // 
            // 2. TextBox: Nhập ID (Vị trí Y: 165)
            // 
            this.txtRoomId.Font = new System.Drawing.Font("Times New Roman", 16F, System.Drawing.FontStyle.Regular);
            this.txtRoomId.Location = new System.Drawing.Point(centerX, 165);
            this.txtRoomId.Multiline = true;
            this.txtRoomId.Name = "txtRoomId";
            this.txtRoomId.Size = new System.Drawing.Size(btnWidth, 40);
            this.txtRoomId.TabIndex = 1;
            this.txtRoomId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRoomId.PlaceholderText = "Nhập ID Phòng";

            // 
            // 3. Button: Vào Phòng (Vị trí Y: 215 - Dính liền dưới TextBox 10px)
            // 
            this.btnJoinRoom.AutoSize = false;
            this.btnJoinRoom.BackColor = btnColor;
            this.btnJoinRoom.Font = mainFont;
            this.btnJoinRoom.ForeColor = txtColor;
            this.btnJoinRoom.Location = new System.Drawing.Point(centerX, 215);
            this.btnJoinRoom.Name = "btnJoinRoom";
            this.btnJoinRoom.Size = new System.Drawing.Size(btnWidth, btnHeight);
            this.btnJoinRoom.TabIndex = 2;
            this.btnJoinRoom.Text = "Vào Phòng";
            this.btnJoinRoom.UseVisualStyleBackColor = false;
            this.btnJoinRoom.Click += new System.EventHandler(this.btnJoinRoom_Click);

            // 
            // --- CỤM TẠO PHÒNG (Cách cụm trên 50px) ---
            //

            // 
            // 4. Button: Tạo Phòng (Vị trí Y: 320)
            // 
            this.btnCreateRoom.AutoSize = false;
            this.btnCreateRoom.BackColor = btnColor;
            this.btnCreateRoom.Font = mainFont;
            this.btnCreateRoom.ForeColor = txtColor;
            this.btnCreateRoom.Location = new System.Drawing.Point(centerX, 320);
            this.btnCreateRoom.Name = "btnCreateRoom";
            this.btnCreateRoom.Size = new System.Drawing.Size(btnWidth, btnHeight);
            this.btnCreateRoom.TabIndex = 3;
            this.btnCreateRoom.Text = "Tạo Phòng";
            this.btnCreateRoom.UseVisualStyleBackColor = false;
            this.btnCreateRoom.Click += new System.EventHandler(this.btnCreateRoom_Click);

            // 
            // Label: Hiển thị ID (Ngay dưới nút Tạo phòng)
            // 
            this.labelRoom.AutoSize = false;
            this.labelRoom.BackColor = System.Drawing.Color.Transparent;
            this.labelRoom.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.labelRoom.ForeColor = System.Drawing.Color.Yellow;
            this.labelRoom.Location = new System.Drawing.Point(centerX, 380);
            this.labelRoom.Name = "labelRoom";
            this.labelRoom.Size = new System.Drawing.Size(btnWidth, 30);
            this.labelRoom.TabIndex = 4;
            this.labelRoom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // --- CÁC NÚT Ở DƯỚI ĐÁY ---
            // 

            // 
            // Button: Bạn bè (Góc trái dưới - Cách lề trái 50px)
            // 
            this.btnFriend.AutoSize = false;
            this.btnFriend.BackColor = btnColor;
            this.btnFriend.Font = mainFont;
            this.btnFriend.ForeColor = txtColor;
            this.btnFriend.Location = new System.Drawing.Point(50, 440);
            this.btnFriend.Name = "btnFriend";
            this.btnFriend.Size = new System.Drawing.Size(180, 55);
            this.btnFriend.TabIndex = 5;
            this.btnFriend.Text = "Bạn bè";
            this.btnFriend.UseVisualStyleBackColor = false;
            this.btnFriend.Click += new System.EventHandler(this.btnFriend_Click);

            // 
            // Button: Đăng Xuất (Góc phải dưới - Cách lề phải 50px)
            // X = 962 - 50 - 180 = 732
            // 
            this.button4.AutoSize = false;
            this.button4.BackColor = btnColor;
            this.button4.Font = mainFont;
            this.button4.ForeColor = txtColor;
            this.button4.Location = new System.Drawing.Point(732, 440);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(180, 55);
            this.button4.TabIndex = 6;
            this.button4.Text = "Đăng Xuất";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);

            // 
            // Controls ẩn/cũ
            // 
            this.button2.Visible = false;
            this.button2.Location = new System.Drawing.Point(0, 0);

            this.button3.Visible = false;
            this.button3.Location = new System.Drawing.Point(0, 0);

            // 
            // MainMenu Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = Properties.Resources.BackGround;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(962, 521);

            // Khóa thay đổi kích thước Form để không bị vỡ layout
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.Controls.Add(this.labelRoom);
            this.Controls.Add(this.btnCreateRoom);
            this.Controls.Add(this.btnJoinRoom);
            this.Controls.Add(this.txtRoomId);
            this.Controls.Add(this.btnFriend);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button1);
            // Ẩn
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);

            this.Name = "MainMenu";
            this.Text = "Game Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
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
    }
}