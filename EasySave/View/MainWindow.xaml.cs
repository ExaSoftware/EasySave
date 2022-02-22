using System.Windows;

namespace EasySave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            frame.Navigate(new MainView());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(SettingsView.GetInstance());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainViewModel.StopConnection();
        }
    }
}
