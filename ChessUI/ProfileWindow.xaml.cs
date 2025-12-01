using ChessData;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChessUI
{
    public partial class ProfileWindow : Window
    {
        private readonly string _username;
        private readonly string _connection;

        public ProfileWindow(string username, string connectionString)
        {
            InitializeComponent();
            _username = username;
            _connection = connectionString;

            LoadProfile();
        }

        private async void LoadProfile()
        {
            var repo = new UserRepository(_connection);
            var stats = await repo.GetUserStatsAsync(_username);

            if (stats == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu người chơi.");
                this.Close();
                return;
            }

            // Avatar
            lblAvatarLetter.Text = stats.IngameName.Substring(0, 1).ToUpper();

            // Ingame name
            lblIngameName.Text = stats.IngameName;
            lblRank.Text = stats.Rank.ToString();
            lblHighestRank.Text = stats.HighestRank.ToString();
            lblWins.Text = stats.Wins.ToString();
            lblLosses.Text = stats.Losses.ToString();

            // Winrate
            double winRate = stats.Wins + stats.Losses == 0
                ? 0
                : (double)stats.Wins / (stats.Wins + stats.Losses);

            lblWinRate.Text = winRate.ToString("0.##") + "%";

            // Total time
            TimeSpan t = TimeSpan.FromMinutes(stats.TotalPlayTimeMinutes);
            lblTotalTime.Text = t.ToString(@"hh\:mm\:ss");

            // Rank → Title
            string title = GetTitleByRank(stats.Rank);
            lblTitle.Text = title;
            lblTitle.Foreground = GetTitleColor(title);
        }

        private string GetTitleByRank(int rank)
        {
            if (rank < 1000) return "Beginner";
            if (rank < 1200) return "Bronze";
            if (rank < 1500) return "Silver";
            if (rank < 1800) return "Gold";
            if (rank < 2200) return "Platinum";
            if (rank < 2600) return "Diamond";
            return "Master";
        }

        private SolidColorBrush GetTitleColor(string title)
        {
            return title switch
            {
                "Beginner" => new SolidColorBrush(Colors.Gray),
                "Bronze" => new SolidColorBrush(Color.FromRgb(205, 127, 50)),
                "Silver" => new SolidColorBrush(Color.FromRgb(192, 192, 192)),
                "Gold" => new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                "Platinum" => new SolidColorBrush(Color.FromRgb(0, 255, 255)),
                "Diamond" => new SolidColorBrush(Color.FromRgb(0, 255, 210)),
                "Master" => new SolidColorBrush(Color.FromRgb(255, 0, 255)),
                _ => new SolidColorBrush(Colors.White)
            };
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
