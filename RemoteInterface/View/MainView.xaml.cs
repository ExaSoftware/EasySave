using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RemoteInterface
{
    /// <summary>
    /// Logique d'interaction pour DetailView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        private MainViewModel _mainViewModel;
        private int id;
        /// <summary>
        /// Constructor of the view
        /// </summary>
        public MainView()
        {
            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// A user click two time on a job backup to show details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Method which was excuted when user select a job backup in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewBackups_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Prevent click on the empty list to avoid an exception
            if (listViewBackups.SelectedItems.Count != 0)
            {

                //Update the first block
                id = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].Id;

                //Get the totalFileSize from the VM
                _mainViewModel.TotalFilesSizeFormatted = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].TotalFileSize();

                ResourceManager rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
                _mainViewModel.JobTypeFormatted = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].IsDifferential ? rm.GetString("differential") : rm.GetString("total");
            }
        }

        /// <summary>
        /// Method which start the all job backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExecuteSequentially_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ExecuteAll(listViewBackups.SelectedItems.Cast<JobBackup>().ToList());
        }

        /// <summary>
        /// Method which is executed if the user wants to connect the remote app with EasySave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoteConnection_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.CommunicationWithRemoteInterface();
        }

        /// <summary>
        /// Method which start the selected job backup when user click on the play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            //If there is a job backup selected
            if (listViewBackups.SelectedItems.Count != 0)
            {
                _mainViewModel.ExecuteAll(listViewBackups.SelectedItems.Cast<JobBackup>().ToList());
            }
        }

        /// <summary>
        /// Turns all running JobBackup in pause state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Stop all running JobBackup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //If there is a job backup selected
            if (listViewBackups.SelectedItems.Count != 0)
            {
                _mainViewModel.Stop();
            }

        }

        private void listViewBackups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mainViewModel.SelectedIndex != -1)
            {
                //MessageBox.Show(_mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].Label);
                MainViewModel vm = this.DataContext as MainViewModel;
                vm.Job = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex];
            }
        }
    }
}
