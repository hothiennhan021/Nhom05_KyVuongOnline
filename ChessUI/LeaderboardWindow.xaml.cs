using ChessClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ChessUI
{
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardVM VM { get; } = new LeaderboardVM();

        public LeaderboardWindow()
        {
            InitializeComponent();
            DataContext = VM;
            LoadLeaderboard();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadLeaderboard();

        private async void LoadLeaderboard()
        {
            try
            {
                if (!ClientManager.Instance.IsConnected)
                {
                    MessageBox.Show("Mất kết nối server");
                    return;
                }

                await ClientManager.Instance.SendAsync("LEADERBOARD_GET|20");

                // Chờ server trả
                string resp = ClientManager.Instance.WaitForMessage();
                if (string.IsNullOrEmpty(resp) || !resp.StartsWith("LEADERBOARD|"))
                    return;

                var rows = ParseLeaderboard(resp);

                // Gán rank + sort Elo giảm dần
                var ranked = rows
                    .OrderByDescending(r => r.Elo)
                    .Select((r, i) =>
                    {
                        r.Rank = i + 1;
                        return r;
                    })
                    .ToList();

                VM.SetRows(ranked);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Leaderboard error: " + ex.Message);
            }
        }

        private void Row_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.FrameworkElement fe) return;
            if (fe.DataContext is not LeaderboardRow row) return;

            try
            {
                var win = new ChessUI.ProfileWindow(row.Username);
                win.Owner = this;      // cho nó nổi lên đúng cửa sổ
                win.ShowDialog();      // ProfileWindow tự gọi GET_PROFILE|username + GET_AVATAR|username
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được hồ sơ người chơi. Lỗi: " + ex.Message);
            }
        }


        private List<LeaderboardRow> ParseLeaderboard(string msg)
        {
            // LEADERBOARD|u1,1500,10,5;u2,1400,8,6
            var list = new List<LeaderboardRow>();

            var payload = msg.Split('|')[1];
            var items = payload.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var it in items)
            {
                var p = it.Split(',');
                if (p.Length < 4) continue;

                list.Add(new LeaderboardRow
                {
                    Username = p[0],
                    Elo = int.Parse(p[1]),
                    Wins = int.Parse(p[2]),
                    Losses = int.Parse(p[3])
                });
            }

            return list;
        }


    }


    public class LeaderboardVM : INotifyPropertyChanged
    {
        public ObservableCollection<LeaderboardRow> Rows { get; } = new();
        public ICollectionView RowsView { get; }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                RowsView.Refresh();
            }
        }

        public string CountText => $"{RowsView.Cast<object>().Count()} players";

        public LeaderboardVM()
        {
            RowsView = CollectionViewSource.GetDefaultView(Rows);
            RowsView.Filter = o =>
            {
                if (o is not LeaderboardRow r) return false;
                if (string.IsNullOrWhiteSpace(SearchText)) return true;
                return r.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            };
        }

        public void SetRows(IEnumerable<LeaderboardRow> rows)
        {
            Rows.Clear();
            foreach (var r in rows) Rows.Add(r);

            RowsView.Refresh();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CountText)));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class LeaderboardRow
    {
        public int Rank { get; set; }
        public string Username { get; set; } = "";
        public int Elo { get; set; }

        public int Wins { get; set; }
        public int Losses { get; set; }

        public int Matches => Wins + Losses;
        public double WinRate => Matches == 0 ? 0 : (double)Wins / Matches;
        public string WinRateText => $"{WinRate * 100:0.#}%";
    }
}
