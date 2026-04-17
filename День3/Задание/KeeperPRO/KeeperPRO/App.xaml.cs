using System.Configuration;
using System.Windows;

namespace KeeperPRO
{
    public partial class App : Application
    {
        public static string ConnectionString { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConnectionString = ConfigurationManager.ConnectionStrings["KeeperPROConnection"].ConnectionString;
        }
    }
}