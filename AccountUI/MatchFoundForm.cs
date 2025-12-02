#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;

namespace AccountUI
{
    public partial class MatchFoundForm : Form
    {
        // ================= CẤU HÌNH =================
        private const int TOTAL_TIME_MS = 15000; // 15 giây
        private int _currentTimeMs = TOTAL_TIME_MS;

        private bool _actionTaken = false;
        private SoundPlayer _musicPlayer;

        // Màu sắc giống trong ảnh
        private readonly Color HexCyan = Color.FromArgb(0, 200, 255); // Màu xanh sáng
        private readonly Color HexBgCircle = Color.FromArgb(20, 30, 45); // Màu nền tối bên trong

        public event Action Accepted;
        public event Action Declined;

        public MatchFoundForm()
        {
            InitializeComponent();

            // Cấu hình Form
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent; // Hiện giữa form cha
            this.DoubleBuffered = true;
            this.BackColor = HexBgCircle;

            // Đảm bảo Form hình vuông để ra hình tròn đẹp
            if (this.Width != this.Height)
            {
                int size = Math.Max(this.Width, this.Height);
                this.Size = new Size(size, size);
            }

            // --- CẮT FORM THÀNH HÌNH TRÒN (QUAN TRỌNG) ---
            try
            {
                GraphicsPath path = new GraphicsPath();
                // Cắt toàn bộ form thành hình tròn
                path.AddEllipse(0, 0, this.Width, this.Height);
                this.Region = new Region(path);
            }
            catch { }

            // Phát nhạc
            PlaySoundSafe();

            // Timer chạy nhanh (50ms) để vòng tròn xoay mượt
            try
            {
                if (timerCountdown != null)
                {
                    timerCountdown.Interval = 50;
                    timerCountdown.Tick -= timerCountdown_Tick;
                    timerCountdown.Tick += timerCountdown_Tick;
                    timerCountdown.Start();
                }
            }
            catch { }
        }

        // --- QUẢN LÝ ÂM THANH ---
        private void PlaySoundSafe()
        {
            try
            {
                var stream = Properties.Resources.MatchFound;
                if (stream != null)
                {
                    stream.Position = 0;
                    _musicPlayer = new SoundPlayer(stream);
                    _musicPlayer.Play();
                }
            }
            catch { }
        }

        private void StopMusic()
        {
            try
            {
                if (_musicPlayer != null)
                {
                    _musicPlayer.Stop();
                    _musicPlayer.Dispose();
                    _musicPlayer = null;
                }
            }
            catch { }
        }

        // --- VẼ GIAO DIỆN (VẼ VÒNG TRÒN PROGRESS) ---
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Bật khử răng cưa để hình tròn mịn
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int thickness = 8; // Độ dày viền xanh
            // Tính toán để vẽ viền sát mép nhưng không bị cắt mất
            int margin = 2;
            int diameter = this.Width - (margin * 2);
            int x = margin;
            int y = margin;

            // 1. Vẽ viền mờ (Background Ring)
            using (Pen p = new Pen(Color.FromArgb(50, 255, 255, 255), thickness)) // Màu trắng mờ
            {
                // Thụt vào 1 chút để bằng với vòng xanh
                e.Graphics.DrawEllipse(p, x + thickness / 2, y + thickness / 2, diameter - thickness, diameter - thickness);
            }

            // 2. Vẽ vòng xanh chạy (Progress Ring)
            float percent = Math.Max(0, (float)_currentTimeMs / TOTAL_TIME_MS);
            float sweepAngle = percent * 360f;

            using (Pen p = new Pen(HexCyan, thickness))
            {
                p.StartCap = LineCap.Round; // Đầu bo tròn
                p.EndCap = LineCap.Round;   // Đuôi bo tròn

                // Vẽ cung tròn (-90 là bắt đầu từ đỉnh 12h)
                // rect phải trừ đi độ dày bút vẽ để nằm gọn trong form
                e.Graphics.DrawArc(p, x + thickness / 2, y + thickness / 2, diameter - thickness, diameter - thickness, -90, sweepAngle);
            }
        }

        private void timerCountdown_Tick(object sender, EventArgs e)
        {
            if (_actionTaken) return;

            _currentTimeMs -= 50;
            this.Invalidate(); // Vẽ lại để vòng tròn xoay

            if (_currentTimeMs <= 0)
            {
                HandleAction(false); // Hết giờ -> Từ chối
            }
        }

        // --- XỬ LÝ NÚT BẤM ---

        private void btnAccept_Click(object sender, EventArgs e)
        {
            HandleAction(true);
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            // Tắt nhạc ngay khi bấm
            StopMusic();

            try { timerCountdown?.Stop(); } catch { }

            MessageBox.Show("Bạn đã từ chối trận đấu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            HandleAction(false);
        }

        private void HandleAction(bool isAccepted)
        {
            if (_actionTaken) return;
            _actionTaken = true;

            StopMusic();
            try { timerCountdown?.Stop(); } catch { }

            try
            {
                if (Controls["btnAccept"] is Button b1) b1.Enabled = false;
                if (Controls["btnDecline"] is Button b2) b2.Enabled = false;
            }
            catch { }

            try
            {
                if (isAccepted)
                {
                    if (Accepted != null) Accepted.Invoke();
                    else this.DialogResult = DialogResult.OK;
                }
                else
                {
                    if (Declined != null) Declined.Invoke();
                    else this.DialogResult = DialogResult.Cancel;
                }
            }
            catch { }
            finally
            {
                this.Close();
            }
        }

        // Hover Effect
        private void btnDecline_MouseEnter(object sender, EventArgs e)
        {
            try { if (sender is Button b) b.ForeColor = Color.Red; } catch { }
        }
        private void btnDecline_MouseLeave(object sender, EventArgs e)
        {
            try { if (sender is Button b) b.ForeColor = Color.Gray; } catch { }
        }
    }
}