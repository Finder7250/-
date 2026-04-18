using System;
using System.Windows;
using KeeperPRO.Department.Data;
using KeeperPRO.Department.Views;

namespace KeeperPRO.Department
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

            if (_repo.LoginDepartmentEmployee(code))
            {
                DepartmentMainWindow departmentWindow = new DepartmentMainWindow(code);
                departmentWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный код сотрудника или вы не привязаны к подразделению!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}