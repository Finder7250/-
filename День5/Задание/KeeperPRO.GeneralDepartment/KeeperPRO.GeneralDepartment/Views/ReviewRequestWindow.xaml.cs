using System;
using System.Data;
using System.Windows;
using KeeperPRO.GeneralDepartment.Data;

namespace KeeperPRO.GeneralDepartment.Views
{
    public partial class ReviewRequestWindow : Window
    {
        private Repository _repo;
        private int _requestId;
        private int _visitorId;
        private bool _isBlacklisted;

        public ReviewRequestWindow(int requestId)
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
            _requestId = requestId;
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
            txtPurpose.Text = row["purpose"]?.ToString() ?? "—";
            txtDepartment.Text = row["department_name"]?.ToString() ?? "—";
            txtEmployee.Text = row["employee_name"]?.ToString() ?? "—";
            txtCreatedAt.Text = row["created_at"] == DBNull.Value ? "—" : Convert.ToDateTime(row["created_at"]).ToString("dd.MM.yyyy HH:mm");

            txtVisitorName.Text = row["visitor_name"]?.ToString() ?? "—";
            txtPassport.Text = row["passport"]?.ToString() ?? "—";
            txtBirthDate.Text = row["birth_date"] == DBNull.Value ? "—" : Convert.ToDateTime(row["birth_date"]).ToString("dd.MM.yyyy");
            txtPhone.Text = row["phone"]?.ToString() ?? "—";
            txtEmail.Text = row["visitor_email"]?.ToString() ?? "—";

            if (row["visitor_id"] != DBNull.Value) _visitorId = Convert.ToInt32(row["visitor_id"]);
            _isBlacklisted = row["is_blacklisted"] != DBNull.Value && Convert.ToBoolean(row["is_blacklisted"]);

            dpVisitDate.SelectedDate = DateTime.Now.AddDays(1);

            if (_isBlacklisted)
            {
                txtBlacklistStatus.Text = "⚠️ ВНИМАНИЕ: Посетитель находится в ЧЕРНОМ СПИСКЕ! Заявка автоматически отклонена.";
                txtBlacklistStatus.Foreground = System.Windows.Media.Brushes.Red;
                gbControls.IsEnabled = false;
                _repo.UpdateRequestStatus(_requestId, "не одобрена", "Посетитель в черном списке", null);
            }
            else
            {
                txtBlacklistStatus.Text = "✅ Проверка пройдена: посетитель не найден в черном списке.";
                txtBlacklistStatus.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void cmbStatus_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool isRejected = (cmbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() == "не одобрена";
            txtRejectionReason.IsEnabled = isRejected;
            lblRejectionReason.Visibility = isRejected ? Visibility.Visible : Visibility.Collapsed;
            txtRejectionReason.Visibility = isRejected ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_isBlacklisted)
            {
                MessageBox.Show("Невозможно изменить статус: посетитель в черном списке!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string status = (cmbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            string rejectionReason = txtRejectionReason.Text;

            if (status == "не одобрена" && string.IsNullOrEmpty(rejectionReason))
            {
                MessageBox.Show("Укажите причину отклонения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = _repo.UpdateRequestStatus(_requestId, status, rejectionReason, dpVisitDate.SelectedDate);

            if (success)
            {
                string message = status == "одобрена"
                    ? $"Заявка одобрена, дата: {dpVisitDate.SelectedDate:dd.MM.yyyy}"
                    : $"Заявка отклонена: {rejectionReason}";
                _repo.SendNotification(_requestId, message);

                MessageBox.Show($"Заявка {status}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}