using System;
using System.Data;
using System.Windows;
using KeeperPRO.GeneralDepartment.Data;
using KeeperPRO.GeneralDepartment.Views;

namespace KeeperPRO.GeneralDepartment.Views
{
    public partial class MainDepartmentWindow : Window
    {
        private Repository _repo;
        private int _employeeId;

        public MainDepartmentWindow(int employeeId)
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
            _employeeId = employeeId;

            LoadDepartmentsFilter();
            LoadRequests();
        }

        private void LoadDepartmentsFilter()
        {
            DataTable dt = _repo.GetDepartments();
            cmbFilterDepartment.Items.Clear();
            cmbFilterDepartment.Items.Add(new { department_id = (int?)null, name = "Все" });
            foreach (DataRow row in dt.Rows)
            {
                cmbFilterDepartment.Items.Add(new { department_id = row["department_id"], name = row["name"] });
            }
            cmbFilterDepartment.DisplayMemberPath = "name";
            cmbFilterDepartment.SelectedValuePath = "department_id";
            cmbFilterDepartment.SelectedIndex = 0;
        }

        private void LoadRequests()
        {
            DataTable dt = _repo.GetAllRequests();
            dgvRequests.ItemsSource = dt.DefaultView;
            dgvRequests.AutoGenerateColumns = true;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            string type = null;
            string typeText = (cmbFilterType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            if (typeText == "Личная") type = "personal";
            else if (typeText == "Групповая") type = "group";

            int? departmentId = null;
            var selectedDept = cmbFilterDepartment.SelectedItem;
            if (selectedDept != null)
            {
                var prop = selectedDept.GetType().GetProperty("department_id");
                if (prop != null) departmentId = prop.GetValue(selectedDept) as int?;
            }

            string status = (cmbFilterStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            if (status == "Все") status = null;

            DataTable dt = _repo.FilterRequests(type, departmentId, status);
            dgvRequests.ItemsSource = dt.DefaultView;
        }

        private void dgvRequests_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgvRequests.SelectedItem != null)
            {
                var row = (DataRowView)dgvRequests.SelectedItem;
                int requestId = Convert.ToInt32(row["request_id"]);

                ReviewRequestWindow reviewWindow = new ReviewRequestWindow(requestId);
                reviewWindow.Owner = this;
                reviewWindow.ShowDialog();
                LoadRequests();
            }
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsWindow reportsWindow = new ReportsWindow();
            reportsWindow.Owner = this;
            reportsWindow.ShowDialog();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}