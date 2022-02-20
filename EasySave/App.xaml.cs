using System.Diagnostics;
using System.Windows;

namespace EasySave
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Configuration _configuration;

        private static bool _threadPause;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Single instance of EasySave
            Process easySave = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(easySave.ProcessName).Length > 1)
            {
                MessageBox.Show("An instance of EasySave is already running...", "Instance Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
            else
            {
                Init.CreateDataDirectoryIfNotExists();
                _configuration = Init.LoadConfiguration();
            }
        }

        public static Configuration Configuration { get => _configuration; }
        public static bool ThreadPause { get => _threadPause; set => _threadPause = value; }
    }
}
