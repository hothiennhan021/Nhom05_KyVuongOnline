using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AccountUI
{
    public class RoundedPanel : Panel
    {
        /// <summary>
        /// Bán kính bo góc (giống logic RoundControl trong Login)
        /// </summary>
        public int CornerRadius { get; set; } = 26;

        public RoundedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.FromArgb(24, 24, 32);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundedRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Vẽ nền + nội dung mặc định
            base.OnPaint(e);

            // Vẽ viền mảnh theo đúng hình bo góc
            if (CornerRadius > 0 && Width > 0 && Height > 0)
            {
                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

                using (GraphicsPath path = GetRoundedPath(rect, CornerRadius))
                using (Pen pen = new Pen(Color.FromArgb(40, Color.White), 1f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Áp dụng Region bo góc cho panel (y chang RoundControl trong Login)
        /// </summary>
        private void ApplyRoundedRegion()
        {
            if (CornerRadius <= 0 || Width <= 0 || Height <= 0)
            {
                Region = new Region(new Rectangle(0, 0, Width, Height));
                return;
            }

            Rectangle rect = new Rectangle(0, 0, Width, Height);
            using (GraphicsPath path = GetRoundedPath(rect, CornerRadius))
            {
                Region = new Region(path);
            }
        }

        /// <summary>
        /// Tạo GraphicsPath bo tròn 4 góc, cùng thứ tự AddArc như trong Login.RoundControl
        /// </summary>
        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                path.CloseAllFigures();
                return path;
            }

            int d = radius * 2;

            // Giống thứ tự:
            // 1. Góc trên trái  (180 -> 270)
            // 2. Góc trên phải (270 -> 360)
            // 3. Góc dưới phải (  0 ->  90)
            // 4. Góc dưới trái ( 90 -> 180)
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);

            path.CloseAllFigures();
            return path;
        }
    }
}
