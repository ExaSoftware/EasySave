using EasySave.ViewModel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasySave
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
            DataContext = _mainViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Method which was executed when a user click on delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteJob_Click(object sender, RoutedEventArgs e)
        {
            if (listViewBackups.SelectedItems.Count != 0)
            {
                _mainViewModel.DeleteSave(id);
            }
        }
        private void btnAddJob_Click(object sender, RoutedEventArgs e)
        {
            //When we want add a job backup, there is no need to pass a job as parameter
            CreateJobView addJobView = new CreateJobView(new CreateJobViewModel(_mainViewModel.ListOfJobBackup));
            this.NavigationService.Navigate(addJobView);
        }

        /// <summary>
        /// A user click two time on a job backup to show details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Prevent click on the empty list to avoid an exception
            if (listViewBackups.SelectedItems.Count != 0 && !_mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].IsRunning)
            {
                CreateJobView editJobView = new CreateJobView(new CreateJobViewModel((JobBackup)listViewBackups.SelectedItem, _mainViewModel.ListOfJobBackup));
                this.NavigationService.Navigate(editJobView);
            }
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

                _mainViewModel.SetTotalFileSize(_mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex]);

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
            _mainViewModel.SortList(listViewBackups.Items.Cast<JobBackup>().ToList());
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
                //Communication.SendTest();
                _mainViewModel.SortList(listViewBackups.SelectedItems.Cast<JobBackup>().ToList());
            }
        }

        /// <summary>
        /// Turns all running JobBackup in pause state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Pause();
        }

        /// <summary>
        /// Stop all running JobBackup.
        /// </summary>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Stop();
        }

        private void listViewBackups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mainViewModel.SelectedIndex != -1)
            {
                //MessageBox.Show(_mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].Label);
                MainViewModel vm = DataContext as MainViewModel;
                vm.Job = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex];
            }
        }
    }
}
