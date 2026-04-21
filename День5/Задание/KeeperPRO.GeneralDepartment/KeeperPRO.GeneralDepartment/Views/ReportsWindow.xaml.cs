using System;
using System.Data;
using System.Windows;
using KeeperPRO.GeneralDepartment.Data;

namespace KeeperPRO.GeneralDepartment.Views
{
    public partial class ReportsWindow : Window
    {
        private Repository _repo;

        public ReportsWindow()
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
        }

        private void btnLoadStats_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string period = ((System.Windows.Controls.ComboBoxItem)cmbPeriod.SelectedItem).Content.ToString();
                string periodEn = period == "День" ? "day" : (period == "Месяц" ? "month" : "year");

                DataTable dt = _repo.GetVisitsCountReport(periodEn);
                dgvStats.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}