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

        Socket _client;
        Socket _newsock;
        /// <summary>
        /// Constructor of the view
        /// </summary>
        public MainView()
        {
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
            InitializeComponent();
            //Creation of a communication point between the local IP address and the port
            /*IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2222);

            // Creation of the socket to connect to the port
            _newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the communication point
            _newsock.Bind(ipep);

            //Create a new thread that listen the network and accept the client request
            Thread t = new Thread(new ThreadStart(ListenNetwork));
            t.Start();*/
        }

        /*private void ListenNetwork()
        {
            _newsock.Listen(1);
            _client = _newsock.Accept();
            IPEndPoint socketEndPoint = (IPEndPoint)_client.RemoteEndPoint;
            Trace.WriteLine(String.Format("Client connected: {0}:{1}", socketEndPoint.Address, socketEndPoint.Port));
            while (true)
            {
                byte[] data = new byte[128];

                //appel de la méthode receive qui reçoit les données envoyées par le serveur et les stocker 
                //dans le tableau data, elle renvoie le nombre d'octet reçus
                try
                {
                    int recv = _client.Receive(data);
                }
                catch (SocketException exception)
                {
                    Trace.WriteLine(exception.Message);
                }


                //transcodage de data en string
                String mg = (Encoding.UTF8.GetString(data));
                //affichage des données recues dans le label label1
                Trace.WriteLine(mg);
            }

        }*/

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
            /*try
            {
                _client.Send(Encoding.UTF8.GetBytes("test"));
            }
            catch (SocketException exp)
            {
                Trace.WriteLine(exp.Message);

            }*/
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
