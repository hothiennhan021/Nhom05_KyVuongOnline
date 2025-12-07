using ChessClient;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChessUI
{
    public partial class SelectAvatarWindow : Window
    {
        private readonly string _username;

        public SelectAvatarWindow(string username)
        {
            InitializeComponent();
            _username = username;
        }

        private async void Avatar_Click(object sender, MouseButtonEventArgs e)
        {
            var img = sender as System.Windows.Controls.Image;
            if (img == null || img.Source == null)
            {
                MessageBox.Show("Không đọc được ảnh!");
                return;
            }

            BitmapSource bmp = img.Source as BitmapSource;
            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(ms);
                bytes = ms.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);

            await ClientManager.Instance.SendAsync($"SET_AVATAR|{_username}|{base64}");
            string resp = ClientManager.Instance.WaitForMessage();

            if (resp == "SET_AVATAR_OK")
                MessageBox.Show("Avatar đã được cập nhật!");

            Close();
        }
    }
}
