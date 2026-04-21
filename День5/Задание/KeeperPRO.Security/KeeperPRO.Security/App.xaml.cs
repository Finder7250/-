using System.Configuration;
using System.Windows;

namespace KeeperPRO.Security
{
    public partial class App : Application
    {
        public static string ConnectionString { get; private set; }

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
        }
    }
}