using System.Windows;

namespace ChessUI
{
    public partial class ChangeIngameNameDialog : Window
    {
        public string NewName { get; private set; }

        public ChangeIngameNameDialog(string current)
        {
            InitializeComponent();
            txtIngame.Text = current;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIngame.Text))
            {
                MessageBox.Show("Tên không được để trống!");
                return;
            }

            NewName = txtIngame.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
