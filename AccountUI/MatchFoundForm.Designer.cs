namespace AccountUI
{
    partial class MatchFoundForm
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
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSub = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnDecline = new System.Windows.Forms.Button();
            this.timerCountdown = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();

            // 
            // lblTitle (TIÊU ĐỀ: TÌM THẤY TRẬN)
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 110);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(350, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TÌM THẤY TRẬN"; // Đã Việt hóa
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lblSub (PHỤ ĐỀ: Kỳ Vương • Xếp Hạng)
            // 
            this.lblSub.BackColor = System.Drawing.Color.Transparent;
            this.lblSub.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblSub.Location = new System.Drawing.Point(0, 150);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(350, 20);
            this.lblSub.TabIndex = 1;
            this.lblSub.Text = "Kỳ Vương • Xếp Hạng"; // Đã Việt hóa và đồng bộ tên game
            this.lblSub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // btnAccept (NÚT CHẤP NHẬN)
            // 
            this.btnAccept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.btnAccept.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAccept.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnAccept.FlatAppearance.BorderSize = 2;
            this.btnAccept.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnAccept.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(60)))), ((int)(((byte)(80)))));
            this.btnAccept.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccept.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAccept.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnAccept.Location = new System.Drawing.Point(95, 220);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(160, 45);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "CHẤP NHẬN!"; // Đã Việt hóa
            this.btnAccept.UseVisualStyleBackColor = false;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);

            // 
            // btnDecline (NÚT TỪ CHỐI)
            // 
            this.btnDecline.BackColor = System.Drawing.Color.Transparent;
            this.btnDecline.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDecline.FlatAppearance.BorderSize = 0;
            this.btnDecline.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnDecline.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnDecline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDecline.Font = new System.Drawing.Font("Segoe UI", 9F); // Tăng size font lên 9 cho dễ đọc
            this.btnDecline.ForeColor = System.Drawing.Color.Gray;
            this.btnDecline.Location = new System.Drawing.Point(125, 275);
            this.btnDecline.Name = "btnDecline";
            this.btnDecline.Size = new System.Drawing.Size(100, 25);
            this.btnDecline.TabIndex = 3;
            this.btnDecline.Text = "Từ chối"; // Đã Việt hóa
            this.btnDecline.UseVisualStyleBackColor = false;
            this.btnDecline.Click += new System.EventHandler(this.btnDecline_Click);
            this.btnDecline.MouseEnter += new System.EventHandler(this.btnDecline_MouseEnter);
            this.btnDecline.MouseLeave += new System.EventHandler(this.btnDecline_MouseLeave);

            // 
            // timerCountdown
            // 
            this.timerCountdown.Enabled = true;
            this.timerCountdown.Interval = 50;
            this.timerCountdown.Tick += new System.EventHandler(this.timerCountdown_Tick);

            // 
            // MatchFoundForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(350, 350);
            this.Controls.Add(this.btnDecline);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.lblSub);
            this.Controls.Add(this.lblTitle);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MatchFoundForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tìm Thấy Trận"; // Tên Form tiếng Việt
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnDecline;
        private System.Windows.Forms.Timer timerCountdown;
    }
}   