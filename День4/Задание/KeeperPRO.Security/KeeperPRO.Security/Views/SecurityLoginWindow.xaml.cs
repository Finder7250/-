using System;
using System.Windows;
using KeeperPRO.Security.Data;

namespace KeeperPRO.Security.Views
{
    public partial class SecurityLoginWindow : Window
    {
        private Repository _repo;

        public SecurityLoginWindow()
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string codeStr = txtEmployeeCode.Password;

            if (string.IsNullOrEmpty(codeStr) || !int.TryParse(codeStr, out int code))
            {
                MessageBox.Show("Введите корректный код!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_repo.LoginSecurityEmployee(code))
            {
                SecurityMainWindow mainWindow = new SecurityMainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный код сотрудника охраны!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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