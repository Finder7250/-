using System;
using System.Data;
using System.Windows;
using KeeperPRO.Department.Data;

namespace KeeperPRO.Department.Views
{
    public partial class DepartmentMainWindow : Window
    {
        private Repository _repo;
        private int _employeeId;

        public DepartmentMainWindow(int employeeId)
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
            _employeeId = employeeId;

            dpFilterDate.SelectedDate = null;
            LoadRequests();
        }

        private void LoadRequests()
        {
            try
            {
                DataTable dt = _repo.GetApprovedRequestsForDepartment(_employeeId);
                dgvRequests.ItemsSource = dt.DefaultView;
                dgvRequests.AutoGenerateColumns = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime? date = dpFilterDate.SelectedDate;
                DataTable dt = _repo.GetApprovedRequestsForDepartmentFiltered(_employeeId, date);
                dgvRequests.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            dpFilterDate.SelectedDate = null;
            LoadRequests();
        }

        private void dgvRequests_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgvRequests.SelectedItem != null)
            {
                try
                {
                    var row = (DataRowView)dgvRequests.SelectedItem;
                    int requestId = Convert.ToInt32(row["request_id"]);

                    DepartmentRequestWindow requestWindow = new DepartmentRequestWindow(requestId, _employeeId);
                    requestWindow.Owner = this;
                    requestWindow.ShowDialog();
                    LoadRequests();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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