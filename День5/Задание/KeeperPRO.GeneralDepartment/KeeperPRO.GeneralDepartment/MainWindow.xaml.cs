using System;
using System.Windows;
using KeeperPRO.GeneralDepartment.Data;
using KeeperPRO.GeneralDepartment.Views;

namespace KeeperPRO.GeneralDepartment
{
    public partial class MainWindow : Window
    {
        private Repository _repo;

        public MainWindow()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(App.ConnectionString))
            {
                MessageBox.Show("Ошибка подключения к базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            _repo = new Repository(App.ConnectionString);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string codeStr = txtEmployeeCode.Password;

            if (string.IsNullOrEmpty(codeStr))
            {
                MessageBox.Show("Введите код сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(codeStr, out int code))
            {
                MessageBox.Show("Код должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_repo.LoginEmployee(code))
            {
                MainDepartmentWindow mainWindow = new MainDepartmentWindow(code);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный код сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}