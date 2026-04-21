using System;
using System.Windows;
using KeeperPRO.Department.Data;

namespace KeeperPRO.Department.Views
{
    public partial class DepartmentLoginWindow : Window
    {
        private Repository _repo;

        public DepartmentLoginWindow()
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

            if (_repo.LoginDepartmentEmployee(code))
            {
                DepartmentMainWindow mainWindow = new DepartmentMainWindow(code);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный код сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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