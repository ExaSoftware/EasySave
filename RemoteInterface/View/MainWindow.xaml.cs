using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;
        private static SettingsView _instance;

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            frame.Navigate(new MainView());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(new SettingsView());
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            _mainViewModel.StopConnection();
        }

        //prevent spam click when using of goback()
        public static SettingsView GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingsView();
            }
            return _instance;
        }
    }
}
