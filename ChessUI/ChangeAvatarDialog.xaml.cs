using System.Windows;

namespace ChessUI
{
    public partial class ChangeAvatarDialog : Window
    {
        public string SelectedOption { get; private set; } = null;

        public ChangeAvatarDialog()
        {
            InitializeComponent();
        }

        private void DefaultAvatar_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = "DEFAULT";
            DialogResult = true;
            Close();
        }

        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = "UPLOAD";
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = null;
            DialogResult = false;
            Close();
        }
    }
}
