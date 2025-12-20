using System;
using System.Windows;
using System.Windows.Controls;
using ChessLogic; // Đảm bảo đã Reference project ChessLogic

namespace ChessUI
{
    public partial class GameOverMenu : UserControl
    {
        // Sự kiện để báo ra ngoài cho MainWindow biết
        public event Action<Option> OptionSelected;

        public GameOverMenu()
        {
            InitializeComponent();
        }

        // Hàm hiển thị thông tin (MainWindow sẽ gọi hàm này)
        public void ShowGameOver(string winner, string reason)
        {
            // 1. Xử lý hiển thị Người thắng
            if (winner == "Draw")
            {
                WinnerText.Text = "HÒA CỜ";
                WinnerText.Foreground = System.Windows.Media.Brushes.LightGray;
            }
            else
            {
                string winnerName = (winner == "White") ? "TRẮNG" : "ĐEN";
                WinnerText.Text = $"{winnerName} THẮNG";

                // Đổi màu chữ tùy theo bên thắng (Trắng -> Trắng, Đen -> Xám/Đỏ tùy ý)
                WinnerText.Foreground = (winner == "White")
                    ? System.Windows.Media.Brushes.White
                    : System.Windows.Media.Brushes.Gray;
            }

            // 2. Dịch lý do sang Tiếng Việt
            string vietnameseReason = TranslateReason(reason);
            ReasonText.Text = vietnameseReason;

            // 3. Reset trạng thái nút Chơi lại
            if (BtnRestart != null)
            {
                BtnRestart.IsEnabled = true;
                BtnRestart.Content = "CHƠI LẠI";
                BtnRestart.Opacity = 1.0;
            }
        }

        // Hàm phụ trợ để dịch các thuật ngữ cờ vua
        private string TranslateReason(string reason)
        {
            switch (reason)
            {
                case "Checkmate":
                    return "Chiếu bí";
                case "Stalemate":
                    return "Hòa pat (Hết nước đi)";
                case "Resignation":
                case "Resign":
                    return "Đối thủ đầu hàng";
                case "Timeout":
                case "Time Out":
                    return "Hết giờ";
                case "Insufficient Material":
                    return "Không đủ quân chiếu bí";
                case "Threefold Repetition":
                    return "Lặp lại 3 lần";
                case "50-Move Rule":
                    return "Luật 50 nước đi";
                case "Draw Agreement":
                    return "Thỏa thuận hòa";
                default:
                    // Nếu không khớp từ nào thì hiển thị nguyên gốc
                    return reason;
            }
        }

        // Hàm khóa nút (được gọi khi đã bấm Chơi lại để tránh spam)
        public void DisableRestartButton()
        {
            if (BtnRestart != null)
            {
                BtnRestart.IsEnabled = false;
                BtnRestart.Content = "Đang chờ...";
                BtnRestart.Opacity = 0.5;
            }
        }

        // --- CÁC SỰ KIỆN CLICK ---

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Restart);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Exit);
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Analyze);
        }
    }
}