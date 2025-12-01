namespace AccountUI
{
    partial class Friend
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Friend));
            tabControl1 = new TabControl();
            tabList = new TabPage();
            btnRemove = new Button();
            lbFriends = new ListBox();
            btnRefresh = new Button();
            tabSearch = new TabPage();
            btnRefreshRequest = new Button();
            label2 = new Label();
            btnAccept = new Button();
            lbRequests = new ListBox();
            btnSearch = new Button();
            txtSearch = new TextBox();
            label1 = new Label();
            tabControl1.SuspendLayout();
            tabList.SuspendLayout();
            tabSearch.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabList);
            tabControl1.Controls.Add(tabSearch);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.Font = new Font("Segoe UI", 10F);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(553, 615);
            tabControl1.TabIndex = 0;
            tabControl1.DrawItem += tabControl1_DrawItem;
            // 
            // tabList
            // 
            tabList.BackColor = Color.FromArgb(32, 32, 32);
            tabList.Controls.Add(btnRemove);
            tabList.Controls.Add(lbFriends);
            tabList.Controls.Add(btnRefresh);
            tabList.Location = new Point(4, 32);
            tabList.Name = "tabList";
            tabList.Padding = new Padding(3);
            tabList.Size = new Size(545, 579);
            tabList.TabIndex = 0;
            tabList.Text = "Danh sách bạn bè";
            // 
            // btnRemove
            // 
            btnRemove.BackColor = Color.FromArgb(50, 50, 50);
            btnRemove.Cursor = Cursors.Hand;
            btnRemove.FlatAppearance.BorderColor = Color.IndianRed;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRemove.ForeColor = Color.IndianRed;
            btnRemove.Image = (Image)resources.GetObject("btnRemove.Image");
            btnRemove.ImageAlign = ContentAlignment.MiddleLeft;
            btnRemove.Location = new Point(8, 448);
            btnRemove.Name = "btnRemove";
            btnRemove.Padding = new Padding(10, 0, 10, 0);
            btnRemove.Size = new Size(526, 52);
            btnRemove.TabIndex = 3;
            btnRemove.Text = "Hủy kết bạn / Xóa";
            btnRemove.UseVisualStyleBackColor = false;
            btnRemove.Click += btnRemove_Click;
            // 
            // lbFriends
            // 
            lbFriends.BackColor = Color.FromArgb(45, 45, 48);
            lbFriends.BorderStyle = BorderStyle.FixedSingle;
            lbFriends.Font = new Font("Segoe UI", 11F);
            lbFriends.ForeColor = Color.WhiteSmoke;
            lbFriends.FormattingEnabled = true;
            lbFriends.ItemHeight = 25;
            lbFriends.Location = new Point(8, 15);
            lbFriends.Name = "lbFriends";
            lbFriends.Size = new Size(526, 427);
            lbFriends.TabIndex = 2;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(50, 50, 50);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.FlatAppearance.BorderColor = Color.Gold;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.Gold;
            btnRefresh.Image = (Image)resources.GetObject("btnRefresh.Image");
            btnRefresh.ImageAlign = ContentAlignment.MiddleLeft;
            btnRefresh.Location = new Point(8, 506);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Padding = new Padding(20, 0, 20, 5);
            btnRefresh.Size = new Size(526, 65);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Làm mới danh sách";
            btnRefresh.TextImageRelation = TextImageRelation.ImageAboveText;
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click_1;
            // 
            // tabSearch
            // 
            tabSearch.BackColor = Color.FromArgb(32, 32, 32);
            tabSearch.Controls.Add(btnRefreshRequest);
            tabSearch.Controls.Add(label2);
            tabSearch.Controls.Add(btnAccept);
            tabSearch.Controls.Add(lbRequests);
            tabSearch.Controls.Add(btnSearch);
            tabSearch.Controls.Add(txtSearch);
            tabSearch.Controls.Add(label1);
            tabSearch.Location = new Point(4, 32);
            tabSearch.Name = "tabSearch";
            tabSearch.Padding = new Padding(3);
            tabSearch.Size = new Size(545, 579);
            tabSearch.TabIndex = 1;
            tabSearch.Text = "Tìm kiếm & Lời mời";
            // 
            // btnRefreshRequest
            // 
            btnRefreshRequest.BackColor = Color.FromArgb(50, 50, 50);
            btnRefreshRequest.Cursor = Cursors.Hand;
            btnRefreshRequest.FlatAppearance.BorderColor = Color.Silver;
            btnRefreshRequest.FlatStyle = FlatStyle.Flat;
            btnRefreshRequest.Font = new Font("Segoe UI", 9F);
            btnRefreshRequest.ForeColor = Color.White;
            btnRefreshRequest.Location = new Point(398, 108);
            btnRefreshRequest.Name = "btnRefreshRequest";
            btnRefreshRequest.Size = new Size(137, 30);
            btnRefreshRequest.TabIndex = 6;
            btnRefreshRequest.Text = "Làm mới";
            btnRefreshRequest.UseVisualStyleBackColor = false;
            btnRefreshRequest.Click += btnRefreshRequest_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.Silver;
            label2.Location = new Point(9, 115);
            label2.Name = "label2";
            label2.Size = new Size(200, 23);
            label2.TabIndex = 5;
            label2.Text = "Lời mời kết bạn đã nhận:";
            // 
            // btnAccept
            // 
            btnAccept.BackColor = Color.FromArgb(50, 50, 50);
            btnAccept.Cursor = Cursors.Hand;
            btnAccept.FlatAppearance.BorderColor = Color.Gold;
            btnAccept.FlatStyle = FlatStyle.Flat;
            btnAccept.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAccept.ForeColor = Color.Gold;
            btnAccept.Location = new Point(9, 515);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(526, 45);
            btnAccept.TabIndex = 4;
            btnAccept.Text = "Đồng ý kết bạn";
            btnAccept.UseVisualStyleBackColor = false;
            btnAccept.Click += btnAccept_Click;
            // 
            // lbRequests
            // 
            lbRequests.BackColor = Color.FromArgb(45, 45, 48);
            lbRequests.BorderStyle = BorderStyle.FixedSingle;
            lbRequests.Font = new Font("Segoe UI", 11F);
            lbRequests.ForeColor = Color.White;
            lbRequests.FormattingEnabled = true;
            lbRequests.ItemHeight = 25;
            lbRequests.Location = new Point(9, 144);
            lbRequests.Name = "lbRequests";
            lbRequests.Size = new Size(526, 352);
            lbRequests.TabIndex = 3;
            // 
            // btnSearch
            // 
            btnSearch.BackColor = Color.FromArgb(50, 50, 50);
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.FlatAppearance.BorderColor = Color.CornflowerBlue;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSearch.ForeColor = Color.CornflowerBlue;
            btnSearch.Image = (Image)resources.GetObject("btnSearch.Image");
            btnSearch.ImageAlign = ContentAlignment.MiddleLeft;
            btnSearch.Location = new Point(398, 47);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(137, 33);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "Gửi lời mời";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.BackColor = Color.FromArgb(45, 45, 48);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.ForeColor = Color.White;
            txtSearch.Location = new Point(9, 48);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(381, 30);
            txtSearch.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Silver;
            label1.Location = new Point(9, 20);
            label1.Name = "label1";
            label1.Size = new Size(172, 23);
            label1.TabIndex = 0;
            label1.Text = "Nhập tên người chơi:";
            // 
            // Friend
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(553, 615);
            Controls.Add(tabControl1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Friend";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bạn bè";
            Load += FriendForm_Load;
            tabControl1.ResumeLayout(false);
            tabList.ResumeLayout(false);
            tabSearch.ResumeLayout(false);
            tabSearch.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabList;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TabPage tabSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.ListBox lbRequests;
        private System.Windows.Forms.Button btnRefreshRequest;
        private System.Windows.Forms.ListBox lbFriends;
        private System.Windows.Forms.Button btnRemove;
    }
}