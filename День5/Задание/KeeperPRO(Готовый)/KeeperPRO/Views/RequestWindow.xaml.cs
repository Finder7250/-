using System;
using System.Data;
using System.Windows;
using KeeperPRO.Data;
using KeeperPRO.Models;

namespace KeeperPRO.Views
{
    public partial class RequestWindow : Window
    {
        private Repository _repo;
        private int _userId;

        public RequestWindow(int userId)
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
            _userId = userId;

            dpStartDate.SelectedDate = DateTime.Now.AddDays(1);
            dpEndDate.SelectedDate = DateTime.Now.AddDays(2);

            LoadDepartments();
            LoadRequests();
        }

        private void LoadDepartments()
        {
            DataTable dt = _repo.GetDepartments();
            cmbDepartment.DisplayMemberPath = "name";
            cmbDepartment.SelectedValuePath = "department_id";
            cmbDepartment.ItemsSource = dt.DefaultView;
        }

        private void cmbDepartment_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbDepartment.SelectedValue != null)
            {
                int deptId = (int)cmbDepartment.SelectedValue;
                DataTable dt = _repo.GetEmployeesByDepartment(deptId);
                cmbEmployee.DisplayMemberPath = "full_name";
                cmbEmployee.SelectedValuePath = "employee_id";
                cmbEmployee.ItemsSource = dt.DefaultView;
            }
        }

        private void LoadRequests()
        {
            DataTable dt = _repo.GetUserRequests(_userId);
            dgvRequests.ItemsSource = dt.DefaultView;
            dgvRequests.AutoGenerateColumns = true;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = dpStartDate.SelectedDate ?? DateTime.Now.AddDays(1);
            DateTime endDate = dpEndDate.SelectedDate ?? DateTime.Now.AddDays(2);

            if (startDate < DateTime.Now.AddDays(1))
            {
                MessageBox.Show("Дата начала должна быть не ранее следующего дня!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (endDate < startDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (endDate > startDate.AddDays(15))
            {
                MessageBox.Show("Дата окончания не может быть более чем на 15 дней позже даты начала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtPurpose.Text))
            {
                MessageBox.Show("Укажите цель посещения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbDepartment.SelectedValue == null)
            {
                MessageBox.Show("Выберите подразделение!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbEmployee.SelectedValue == null)
            {
                MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string requestType = "personal";
            var selectedItem = cmbRequestType.SelectedItem as System.Windows.Controls.ComboBoxItem;
            if (selectedItem != null && selectedItem.Content.ToString() == "Групповая")
            {
                requestType = "group";
            }

            VisitRequest request = new VisitRequest
            {
                UserId = _userId,
                Type = requestType,
                StartDate = startDate,
                EndDate = endDate,
                Purpose = txtPurpose.Text,
                DepartmentId = (int)cmbDepartment.SelectedValue,
                EmployeeId = (int)cmbEmployee.SelectedValue
            };

            int requestId = _repo.CreateVisitRequest(request);

            if (requestId > 0)
            {
                MessageBox.Show($"Заявка №{requestId} успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadRequests();

                dpStartDate.SelectedDate = DateTime.Now.AddDays(1);
                dpEndDate.SelectedDate = DateTime.Now.AddDays(2);
                txtPurpose.Text = "";
            }
            else
            {
                MessageBox.Show("Ошибка при создании заявки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}