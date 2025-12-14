using ChessClient;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessUI
{
    public partial class ProfileWindow : Window
    {
        private readonly string _username;

        public ProfileWindow(string username)
        {
            InitializeComponent();
            _username = username;

            LoadProfileFromServer();
            LoadAvatarFromServer();
        }

        // ------------------------------ LOAD PROFILE ------------------------------
        private async void LoadProfileFromServer()
        {
            await ClientManager.Instance.SendAsync($"GET_PROFILE|{_username}");
            string resp = ClientManager.Instance.WaitForMessage();

            if (!resp.StartsWith("PROFILE|"))
            {
                MessageBox.Show("Không thể tải thông tin người chơi.");
                Close();
                return;
            }

            var p = resp.Split('|');

            lblIngameName.Text = p[1];
            lblAvatarLetter.Text = p[1].Substring(0, 1).ToUpper();
            lblRank.Text = p[2];
            lblHighestRank.Text = p[3];
            lblWins.Text = p[4];
            lblLosses.Text = p[5];

            int wins = int.Parse(p[4]);
            int losses = int.Parse(p[5]);

            double rate = wins + losses == 0 ? 0 : (wins * 100.0 / (wins + losses));
            lblWinRate.Text = rate.ToString("0.##") + "%";

            TimeSpan t = TimeSpan.FromMinutes(int.Parse(p[6]));
            lblTotalTime.Text = t.ToString(@"hh\:mm\:ss");

            int elo = int.Parse(p[2]);
            string title = GetTitleByRank(elo);
            lblTitle.Text = title;
            lblTitle.Foreground = GetTitleColor(title);
        }

        // ------------------------------ LOAD AVATAR ------------------------------
        private async void LoadAvatarFromServer()
        {
            await ClientManager.Instance.SendAsync($"GET_AVATAR|{_username}");
            string resp = ClientManager.Instance.WaitForMessage();

            if (resp == "AVATAR_NULL")
            {
                AvatarBrush.ImageSource = null;
                lblAvatarLetter.Visibility = Visibility.Visible;
                return;
            }

            if (!resp.StartsWith("AVATAR|")) return;

            try
            {
                string base64 = resp.Substring(7);
                byte[] bytes = Convert.FromBase64String(base64);

                using var ms = new MemoryStream(bytes);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = ms;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                AvatarBrush.ImageSource = bmp;
                lblAvatarLetter.Visibility = Visibility.Collapsed;
            }
            catch
            {
                AvatarBrush.ImageSource = null;
                lblAvatarLetter.Visibility = Visibility.Visible;
            }
        }

        // ------------------------------ CHANGE AVATAR ------------------------------
        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ChangeAvatarDialog();
            dlg.Owner = this;

            if (dlg.ShowDialog() == true)
            {
                if (dlg.SelectedOption == "DEFAULT")
                {
                    var win = new SelectAvatarWindow(_username);
                    win.ShowDialog();
                    LoadAvatarFromServer();
                }
                else if (dlg.SelectedOption == "UPLOAD")
                {
                    UploadAvatarFromPC();
                }
            }
        }

        private async void UploadAvatarFromPC()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (dlg.ShowDialog() != true)
                return;

            try
            {
                byte[] data = File.ReadAllBytes(dlg.FileName);
                string base64 = Convert.ToBase64String(data);

                await ClientManager.Instance.SendAsync($"SET_AVATAR|{_username}|{base64}");

                string resp = ClientManager.Instance.WaitForMessage();

                if (resp == "SET_AVATAR_OK")
                    MessageBox.Show("Cập nhật avatar thành công!");

                LoadAvatarFromServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải avatar:\n" + ex.Message);
            }
        }

        // ⭐⭐⭐ CHANGE INGAME NAME ⭐⭐⭐
        private async void ChangeIngameName_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ChangeIngameNameDialog(lblIngameName.Text);
            dlg.Owner = this;

            if (dlg.ShowDialog() == true)
            {
                string newName = dlg.NewName;

                await ClientManager.Instance.SendAsync($"SET_INGAME|{_username}|{newName}");
                string resp = ClientManager.Instance.WaitForMessage();

                if (resp == "SET_INGAME_OK")
                {
                    MessageBox.Show("Đổi tên In-Game thành công!");
                    LoadProfileFromServer();
                }
                else
                {
                    MessageBox.Show("Không thể đổi tên In-Game!");
                }
            }
        }

        // ------------------------------ STYLE ------------------------------
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
                _ => new SolidColorBrush(Colors.White),
            };
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
