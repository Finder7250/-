using System.Windows;
using KeeperPRO.Data;
using KeeperPRO.Models;

namespace KeeperPRO.Views
{
    public partial class LoginWindow : Window
    {
        private Repository _repo;

        public LoginWindow()
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            User user = _repo.LoginUser(email, password);

            if (user != null)
            {
                MessageBox.Show($"Добро пожаловать, {user.Email}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestWindow requestWindow = new RequestWindow(user.UserId);
                requestWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный email или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}