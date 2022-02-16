using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave
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
    }
}
