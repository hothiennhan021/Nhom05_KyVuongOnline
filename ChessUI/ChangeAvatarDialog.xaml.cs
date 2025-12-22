using System.Windows;

namespace ChessUI
{
    /// <summary>
    /// Cửa sổ Dialog chọn phương thức thay đổi Avatar
    /// </summary>
    public partial class ChangeAvatarDialog : Window
    {
        // Biến lưu lựa chọn của người dùng (DEFAULT hoặc UPLOAD)
        public string SelectedOption { get; private set; } = null;

        public ChangeAvatarDialog()
        {
            InitializeComponent();
        }

        // Xử lý khi chọn Avatar có sẵn
        private void DefaultAvatar_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = "DEFAULT"; // Giữ nguyên từ khóa logic
            DialogResult = true;
            Close();
        }

        // Xử lý khi chọn Tải từ máy
        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = "UPLOAD"; // Giữ nguyên từ khóa logic
            DialogResult = true;
            Close();
        }

        // Xử lý khi bấm Hủy
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = null;
            DialogResult = false;
            Close();
        }
    }
}