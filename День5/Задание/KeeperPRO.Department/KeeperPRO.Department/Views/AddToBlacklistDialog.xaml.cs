using System.Windows;

namespace KeeperPRO.Department.Views
{
    public partial class AddToBlacklistDialog : Window
    {
        public string Reason { get; private set; }

        public AddToBlacklistDialog(string visitorName)
        {
            InitializeComponent();
            txtVisitorName.Text = visitorName;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                MessageBox.Show("Укажите причину добавления в черный список!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtReason.Text.Length > 5000)
            {
                MessageBox.Show("Причина не должна превышать 5000 символов!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Reason = txtReason.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}