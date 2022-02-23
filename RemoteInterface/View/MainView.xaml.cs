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
        /// Method which is executed if the user wants to connect the remote app with EasySave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoteConnection_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.CommunicationWithRemoteInterface();
            //_mainViewModel.receiveContinuously();
        }

        /// <summary>
        /// Method which start the selected job backup when user click on the play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {/*
            //If there is a job backup selected
            if (listViewBackups.SelectedItems.Count != 0)
            {
                _mainViewModel.ExecuteAll(listViewBackups.SelectedItems.Cast<JobBackup>().ToList());
            }*/
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
        {/*
            //If there is a job backup selected
            if (listViewBackups.SelectedItems.Count != 0)
            {
                _mainViewModel.Stop();
            }
        */
        }
    }
}
