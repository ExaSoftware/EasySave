using System.Windows;

namespace EasySave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _ = frame.Navigate(new MainView());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            _ = frame.Navigate(SettingsView.GetInstance());
        }
    }
}