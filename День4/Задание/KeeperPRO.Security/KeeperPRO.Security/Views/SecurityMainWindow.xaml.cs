using System;
using System.Data;
using System.Windows;
using KeeperPRO.Security.Data;

namespace KeeperPRO.Security.Views
{
    public partial class SecurityMainWindow : Window
    {
        private Repository _repo;

        public SecurityMainWindow()
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);

            dpFilterDate.SelectedDate = null;
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
            DataTable dt = _repo.GetApprovedRequests();
            dgvRequests.ItemsSource = dt.DefaultView;
            dgvRequests.AutoGenerateColumns = true;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            DateTime? date = dpFilterDate.SelectedDate;

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

            DataTable dt = _repo.GetApprovedRequestsFiltered(date, type, departmentId);
            dgvRequests.ItemsSource = dt.DefaultView;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadRequests();
                return;
            }

            DataTable dt = _repo.SearchRequests(searchText);
            dgvRequests.ItemsSource = dt.DefaultView;
        }

        private void dgvRequests_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgvRequests.SelectedItem != null)
            {
                var row = (DataRowView)dgvRequests.SelectedItem;
                int requestId = Convert.ToInt32(row["request_id"]);
                int visitorId = row["visitor_id"] != DBNull.Value ? Convert.ToInt32(row["visitor_id"]) : 0;

                AccessWindow accessWindow = new AccessWindow(requestId, visitorId);
                accessWindow.Owner = this;
                accessWindow.ShowDialog();
                LoadRequests();
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