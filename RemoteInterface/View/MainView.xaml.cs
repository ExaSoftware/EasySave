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

namespace RemoteInterface
{
    /// <summary>
    /// Logique d'interaction pour DetailView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        private MainViewModel _mainViewModel;
        private int id;
        static Socket server;
        /// <summary>
        /// Constructor of the view
        /// </summary>
        public MainView()
        {
            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
            InitializeComponent();

            //Creation of a communication point between the local IP address and the port
            /*IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2222);

            // Creation of the socket to connect to the port
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Request of connection
            server.Connect(ipep);

            Thread t = new Thread(new ThreadStart(Listen));
            t.Start();*/
            /*while (true)
            {
                //Déclaration d'un buffer de type byte pour enregistrer les données reçues
                
            }*/
        }
        /*public static void Listen()
        {
            while (true)
            {
                byte[] data = new byte[128];

                //appel de la méthode receive qui reçoit les données envoyées par le serveur et les stocker 
                //dans le tableau data, elle renvoie le nombre d'octet reçus
                try
                {
                    int recv = server.Receive(data);
                }
                catch (SocketException exception)
                {

                }


                //transcodage de data en string
                String mg = (Encoding.UTF8.GetString(data));
                //affichage des données recues dans le label label1
                Trace.WriteLine(mg);
            }
        }*/
            /// <summary>
            /// Method which is executed if the user wants to connect the remote app with EasySave
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void btnRemoteConnection_Click(object sender, RoutedEventArgs e)
        {
            Communication.SendInformation("test depuis remote0");
            /*try
            {

                server.Send(Encoding.UTF8.GetBytes("test depuis remote"));
            }
            catch (SocketException exp)
            {
                Console.WriteLine(exp.Message);

            }*/
            //_mainViewModel.CommunicationWithRemoteInterface();
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
