using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Data;
using KeeperPRO.GeneralDepartment.Data;

namespace KeeperPRO.GeneralDepartment
{
    public partial class App : Application
    {
        public static string ConnectionString { get; private set; }
        private DispatcherTimer _reportTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var connectionStringSetting = ConfigurationManager.ConnectionStrings["KeeperPROConnection"];
            if (connectionStringSetting == null)
            {
                MessageBox.Show("Ошибка: строка подключения не найдена в App.config!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            ConnectionString = connectionStringSetting.ConnectionString;
            StartReportTimer();
        }

        private void StartReportTimer()
        {
            _reportTimer = new DispatcherTimer();
            _reportTimer.Interval = TimeSpan.FromHours(3);
            _reportTimer.Tick += OnReportTimerTick;
            _reportTimer.Start();
        }

        private void OnReportTimerTick(object sender, EventArgs e)
        {
            try
            {
                var repo = new Repository(ConnectionString);
                var dt = repo.GetThreeHourReport();

                string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string reportsFolder = Path.Combine(docsPath, "Отчеты ТБ");
                string dailyFolder = Path.Combine(reportsFolder, DateTime.Now.ToString("dd_MM_yyyy"));

                if (!Directory.Exists(reportsFolder))
                    Directory.CreateDirectory(reportsFolder);
                if (!Directory.Exists(dailyFolder))
                    Directory.CreateDirectory(dailyFolder);

                string fileName = Path.Combine(dailyFolder, $"report_{DateTime.Now:HH_mm}.csv");
                SaveDataTableToCsv(dt, fileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка автоотчета: {ex.Message}");
            }
        }

        private void SaveDataTableToCsv(DataTable dt, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    writer.Write(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1) writer.Write(";");
                }
                writer.WriteLine();

                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        writer.Write(row[i]?.ToString());
                        if (i < dt.Columns.Count - 1) writer.Write(";");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}