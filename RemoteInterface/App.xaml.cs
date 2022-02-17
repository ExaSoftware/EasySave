using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RemoteInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Configuration _configuration;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Single instance of EasySave
            Process remoteInterface = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(remoteInterface.ProcessName).Length > 1)
            {
                MessageBox.Show("An instance of the remote interface is already running...", "Instance Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
            else
            {
                _configuration = Init.LoadConfiguration();
            }
        }

        public static Configuration Configuration { get => _configuration; }
    }
}
