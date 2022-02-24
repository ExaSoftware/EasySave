using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        private static bool _isMovingBigFile;

        public static Configuration Configuration { get => _configuration; }
        public static bool ThreadPause { get => _threadPause; set => _threadPause = value; }
        public static bool IsMovingBigFile { get => _isMovingBigFile; set => _isMovingBigFile = value; }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Single instance of EasySave
            Process easySave = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(easySave.ProcessName).Length > 1)
            {
                MessageBox.Show("An instance of EasySave is already running...", "Instance Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
            else
            {
                Init.CreateDataDirectoryIfNotExists();
                _configuration = Init.LoadConfiguration();
            }

            //Begin thread to monitor the business software
            Task.Run(() =>
            {
                const int time = 1000;

                while (true)
                {
                    try
                    {
                        if (Configuration.BusinessSoftware != "" || Configuration.BusinessSoftware != null)
                        {
                            if (ThreadPause)
                            {
                                Thread.Sleep(time);
                            }
                            else
                            {
                                ThreadPause = Process.GetProcessesByName(Configuration.BusinessSoftware).Length != 0;
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        Thread.Sleep(time);
                    }
                }
            });
        }

        protected override void OnExit(ExitEventArgs e) => Environment.Exit(0);

    }
}