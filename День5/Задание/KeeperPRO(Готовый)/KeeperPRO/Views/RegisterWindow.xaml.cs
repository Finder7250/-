using System;
using System.Text.RegularExpressions;
using System.Windows;
using KeeperPRO.Data;

namespace KeeperPRO.Views
{
    public partial class RegisterWindow : Window
    {
        private Repository _repo;

        public RegisterWindow()
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
        }

        private bool IsValidPassword(string password)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+=\[{\]};:<>|./?,-]).{8,}$");
            return regex.IsMatch(password);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidPassword(password))
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов, цифру, спецсимвол, буквы в верхнем и нижнем регистре!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _repo.RegisterUser(email, password);

            if (result)
            {
                MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Пользователь с таким email уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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