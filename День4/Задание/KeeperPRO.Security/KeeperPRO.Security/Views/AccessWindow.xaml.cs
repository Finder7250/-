using System;
using System.Data;
using System.Windows;
using KeeperPRO.Security.Data;

namespace KeeperPRO.Security.Views
{
    public partial class AccessWindow : Window
    {
        private Repository _repo;
        private int _requestId;
        private int _visitorId;

        public AccessWindow(int requestId, int visitorId)
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
            _requestId = requestId;
            _visitorId = visitorId;
            LoadRequestData();
        }

        private void LoadRequestData()
        {
            DataRow row = _repo.GetRequestById(_requestId);
            if (row == null)
            {
                MessageBox.Show("Заявка не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            txtType.Text = row["type"]?.ToString() == "personal" ? "Личная" : "Групповая";
            txtDepartment.Text = row["department_name"]?.ToString() ?? "—";
            txtVisitDate.Text = row["start_date"] == DBNull.Value ? "—" : Convert.ToDateTime(row["start_date"]).ToString("dd.MM.yyyy");
            txtPurpose.Text = row["purpose"]?.ToString() ?? "—";

            txtVisitorName.Text = row["last_name"]?.ToString() + " " + row["first_name"]?.ToString() + " " + row["middle_name"]?.ToString() ?? "—";
            txtPassport.Text = row["passport"]?.ToString() ?? "—";
            txtPhone.Text = row["phone"]?.ToString() ?? "—";
            txtEmail.Text = row["email"]?.ToString() ?? "—";

            if (row["access_start_time"] != DBNull.Value)
                txtAccessStart.Text = Convert.ToDateTime(row["access_start_time"]).ToString("dd.MM.yyyy HH:mm:ss");
            else
                txtAccessStart.Text = "—";

            if (row["access_end_time"] != DBNull.Value)
                txtAccessEnd.Text = Convert.ToDateTime(row["access_end_time"]).ToString("dd.MM.yyyy HH:mm:ss");
            else
                txtAccessEnd.Text = "—";
        }

        private void btnGrantAccess_Click(object sender, RoutedEventArgs e)
        {
            DateTime accessTime = DateTime.Now;

            bool success = _repo.GrantAccess(_requestId, _visitorId, accessTime);

            if (success)
            {
                _repo.SendOpenGateSignal(_requestId);
                System.Media.SystemSounds.Asterisk.Play();

                txtAccessStatus.Text = $"✓ Доступ разрешен в {accessTime:HH:mm:ss}";
                txtAccessStart.Text = accessTime.ToString("dd.MM.yyyy HH:mm:ss");

                MessageBox.Show($"Доступ разрешен! Время входа: {accessTime:HH:mm:ss}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при разрешении доступа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSetDeparture_Click(object sender, RoutedEventArgs e)
        {
            DateTime departureTime = DateTime.Now;

            bool success = _repo.SetDepartureTime(_requestId, _visitorId, departureTime);

            if (success)
            {
                txtDepartureStatus.Text = $"✓ Выход зафиксирован в {departureTime:HH:mm:ss}";
                txtAccessEnd.Text = departureTime.ToString("dd.MM.yyyy HH:mm:ss");

                MessageBox.Show($"Выход зафиксирован! Время выхода: {departureTime:HH:mm:ss}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при фиксации выхода!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}