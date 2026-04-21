using KeeperPRO.Department.Data;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KeeperPRO.Department.Views
{
    public partial class DepartmentRequestWindow : Window
    {
        private Repository _repo;
        private int _requestId;
        private int _employeeId;

        public DepartmentRequestWindow(int requestId, int employeeId)
        {
            InitializeComponent();
            _repo = new Repository(App.ConnectionString);
            _requestId = requestId;
            _employeeId = employeeId;

            LoadRequestData();
            LoadVisitors();

            dgvVisitors.MouseRightButtonUp += DgvVisitors_MouseRightButtonUp;
        }

        private void LoadRequestData()
        {
            try
            {
                DataRow row = _repo.GetRequestById(_requestId);
                if (row == null)
                {
                    MessageBox.Show("Заявка не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                txtType.Text = row["type"]?.ToString() == "personal" ? "Личная" : "Групповая";
                txtVisitDate.Text = row["start_date"] == DBNull.Value ? "—" : Convert.ToDateTime(row["start_date"]).ToString("dd.MM.yyyy");
                txtPurpose.Text = row["purpose"]?.ToString() ?? "—";
                txtStatus.Text = row["status"]?.ToString() ?? "—";

                txtCheckIn.Text = row["check_in_time"] == DBNull.Value ? "—" : Convert.ToDateTime(row["check_in_time"]).ToString("dd.MM.yyyy HH:mm:ss");
                txtCheckOut.Text = row["check_out_time"] == DBNull.Value ? "—" : Convert.ToDateTime(row["check_out_time"]).ToString("dd.MM.yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVisitors()
        {
            try
            {
                DataTable dt = _repo.GetVisitorsByRequestId(_requestId);
                dgvVisitors.ItemsSource = dt.DefaultView;
                dgvVisitors.AutoGenerateColumns = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списка посетителей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvVisitors_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cellInfo = dgvVisitors.SelectedCells;
                if (cellInfo.Count > 0 && cellInfo[0].Item != null)
                {
                    var row = (DataRowView)cellInfo[0].Item;
                    int visitorId = Convert.ToInt32(row["visitor_id"]);
                    string visitorName = row["last_name"] + " " + row["first_name"] + " " + row["middle_name"];

                    // Проверка, не в черном ли уже списке
                    bool isBlacklisted = row["is_blacklisted"] != DBNull.Value && Convert.ToBoolean(row["is_blacklisted"]);

                    ContextMenu contextMenu = new ContextMenu();
                    MenuItem menuItem = new MenuItem();

                    if (isBlacklisted)
                    {
                        menuItem.Header = $"✓ {visitorName} уже в черном списке";
                        menuItem.IsEnabled = false;
                    }
                    else
                    {
                        menuItem.Header = $"🚫 Добавить в черный список: {visitorName}";
                        menuItem.Click += (s, args) => AddToBlacklist(visitorId, visitorName);
                    }

                    contextMenu.Items.Add(menuItem);
                    dgvVisitors.ContextMenu = contextMenu;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddToBlacklist(int visitorId, string visitorName)
        {
            try
            {
                AddToBlacklistDialog dialog = new AddToBlacklistDialog(visitorName);
                if (dialog.ShowDialog() == true)
                {
                    bool success = _repo.AddToBlacklist(visitorId, dialog.Reason);
                    if (success)
                    {
                        MessageBox.Show($"Посетитель {visitorName} добавлен в черный список!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadVisitors();

                        // Отправка уведомления (эмуляция)
                        _repo.SendBlacklistNotification(visitorId, visitorName, dialog.Reason);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении в черный список!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime checkInTime = DateTime.Now;
                bool success = _repo.SetCheckInTime(_requestId, checkInTime);

                if (success)
                {
                    txtCheckIn.Text = checkInTime.ToString("dd.MM.yyyy HH:mm:ss");
                    MessageBox.Show($"Прибытие зафиксировано в {checkInTime:HH:mm:ss}!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при фиксации прибытия!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка: сначала нужно зафиксировать прибытие
                if (txtCheckIn.Text == "—")
                {
                    MessageBox.Show("Сначала зафиксируйте прибытие посетителя!",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime checkOutTime = DateTime.Now;
                bool success = _repo.SetCheckOutTime(_requestId, checkOutTime);

                if (success)
                {
                    txtCheckOut.Text = checkOutTime.ToString("dd.MM.yyyy HH:mm:ss");
                    MessageBox.Show($"Убытие зафиксировано в {checkOutTime:HH:mm:ss}!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при фиксации убытия!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}