using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RemoteInterface
{
    class MainViewModel : INotifyPropertyChanged
    {
        //Attributes
        private ProgressLog _progressLog;
        private Socket server;

        private string _pl1Name;
        private string _pl2Name;
        private string _pl3Name;
        private string _pl4Name;
        private string _pl5Name;

        private string _pl1TotalFilesToCopy;
        private string _pl2TotalFilesToCopy;
        private string _pl3TotalFilesToCopy;
        private string _pl4TotalFilesToCopy;
        private string _pl5TotalFilesToCopy;

        private string _pl1TotalFilesRemaining;
        private string _pl2TotalFilesRemaining;
        private string _pl3TotalFilesRemaining;
        private string _pl4TotalFilesRemaining;
        private string _pl5TotalFilesRemaining;

        private string _pl1SizeRemaining;
        private string _pl2SizeRemaining;
        private string _pl3SizeRemaining;
        private string _pl4SizeRemaining;
        private string _pl5SizeRemaining;

        private string _pl1State;
        private string _pl2State;
        private string _pl3State;
        private string _pl4State;
        private string _pl5State;

        private string _pl1Progression;
        private string _pl2Progression;
        private string _pl3Progression;
        private string _pl4Progression;
        private string _pl5Progression;

        //Define getter / setter

        public ProgressLog PLog
        {
            get => _progressLog;
            set
            {
                _progressLog = value;
                OnPropertyChanged("ProgressLog");
            }
        }

        public string Pl1Progression { get => _pl1Progression; set { _pl1Progression = value; OnPropertyChanged("Pl1Progression"); } }
        public string Pl2Progression { get => _pl2Progression; set { _pl2Progression = value; OnPropertyChanged("Pl2Progression"); } }
        public string Pl3Progression { get => _pl3Progression; set { _pl3Progression = value; OnPropertyChanged("Pl3Progression"); } }
        public string Pl4Progression { get => _pl4Progression; set { _pl4Progression = value; OnPropertyChanged("Pl4Progression"); } }
        public string Pl5Progression { get => _pl5Progression; set { _pl5Progression = value; OnPropertyChanged("Pl5Progression"); } }

        public string Pl1TotalFilesToCopy { get => _pl1TotalFilesToCopy; set { _pl1TotalFilesToCopy = value; OnPropertyChanged("Pl1TotalFilesToCopy"); } }
        public string Pl2TotalFilesToCopy { get => _pl2TotalFilesToCopy; set { _pl2TotalFilesToCopy = value; OnPropertyChanged("Pl2TotalFilesToCopy"); } }
        public string Pl3TotalFilesToCopy { get => _pl3TotalFilesToCopy; set { _pl3TotalFilesToCopy = value; OnPropertyChanged("Pl3TotalFilesToCopy"); } }
        public string Pl4TotalFilesToCopy { get => _pl4TotalFilesToCopy; set { _pl4TotalFilesToCopy = value; OnPropertyChanged("Pl4TotalFilesToCopy"); } }
        public string Pl5TotalFilesToCopy { get => _pl5TotalFilesToCopy; set { _pl5TotalFilesToCopy = value; OnPropertyChanged("Pl5TotalFilesToCopy"); } }
        public string Pl1TotalFilesRemaining { get => _pl1TotalFilesRemaining; set { _pl1TotalFilesRemaining = value; OnPropertyChanged("Pl1TotalFilesRemaining"); } }
        public string Pl2TotalFilesRemaining { get => _pl2TotalFilesRemaining; set { _pl2TotalFilesRemaining = value; OnPropertyChanged("Pl2TotalFilesRemaining"); } }
        public string Pl3TotalFilesRemaining { get => _pl3TotalFilesRemaining; set { _pl3TotalFilesRemaining = value; OnPropertyChanged("Pl3TotalFilesRemaining"); } }
        public string Pl4TotalFilesRemaining { get => _pl4TotalFilesRemaining; set { _pl4TotalFilesRemaining = value; OnPropertyChanged("Pl4TotalFilesRemaining"); } }
        public string Pl5TotalFilesRemaining { get => _pl5TotalFilesRemaining; set { _pl5TotalFilesRemaining = value; OnPropertyChanged("Pl5TotalFilesRemaining"); } }
        public string Pl1SizeRemaining { get => _pl1SizeRemaining; set { _pl1SizeRemaining = value; OnPropertyChanged("Pl1SizeRemaining"); } }
        public string Pl2SizeRemaining { get => _pl2SizeRemaining; set { _pl2SizeRemaining = value; OnPropertyChanged("Pl2SizeRemaining"); } }
        public string Pl3SizeRemaining { get => _pl3SizeRemaining; set { _pl3SizeRemaining = value; OnPropertyChanged("Pl3SizeRemaining"); } }
        public string Pl4SizeRemaining { get => _pl4SizeRemaining; set { _pl4SizeRemaining = value; OnPropertyChanged("Pl4SizeRemaining"); } }
        public string Pl5SizeRemaining { get => _pl5SizeRemaining; set { _pl5SizeRemaining = value; OnPropertyChanged("Pl5SizeRemaining"); } }
        public string Pl1State { get => _pl1State; set { _pl1State = value; OnPropertyChanged("Pl1State"); } }
        public string Pl2State { get => _pl2State; set { _pl2State = value; OnPropertyChanged("Pl2State"); } }
        public string Pl3State { get => _pl3State; set { _pl3State = value; OnPropertyChanged("Pl3State"); } }
        public string Pl4State { get => _pl4State; set { _pl4State = value; OnPropertyChanged("Pl4State"); } }
        public string Pl5State { get => _pl5State; set { _pl5State = value; OnPropertyChanged("Pl5State"); } }

        public string Pl1Name { get => _pl1Name; set { _pl1Name = value; OnPropertyChanged("Pl1Name"); } }
        public string Pl2Name { get => _pl2Name; set { _pl2Name = value; OnPropertyChanged("Pl2Name"); } }
        public string Pl3Name { get => _pl3Name; set { _pl3Name = value; OnPropertyChanged("Pl3Name"); } }
        public string Pl4Name { get => _pl4Name; set { _pl4Name = value; OnPropertyChanged("Pl4Name"); } }
        public string Pl5Name { get => _pl5Name; set { _pl5Name = value; OnPropertyChanged("Pl5Name"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Constructor of MainViewModel.
        /// </summary>
        public MainViewModel()
        {            

        }

        /// <summary>
        /// Launch the connection to the main application
        /// </summary>
        public void LaunchConnection()
        {

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2222);

            // Creation of the socket to connect to the port
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Request of connection
            server.Connect(ipep);

            Thread t = new Thread(new ThreadStart(Listen));
            t.Start();

        }
        public void Listen()
        {
            while (true)
            {
                byte[] data = new byte[4096];
                try
                {
                    int recv = server.Receive(data);
                }
                catch (SocketException)
                {

                }
                ReceiveProgessLog(data);
            }

        }
        public void ReceiveProgessLog(byte[] data)
        {
            JsonReadWriteModel jsonRWM = new JsonReadWriteModel();
            ProgressLog[] progress = jsonRWM.ReadProgressLog(Encoding.UTF8.GetString(data));
            if (progress[0] != null)
            {
                Pl1Progression = progress[0].Progression.ToString();
                Pl1Name = progress[0].Name;
                Pl1SizeRemaining = progress[0].SizeRemainingFormatted.ToString();
                Pl1State = progress[0].State;
                Pl1TotalFilesRemaining = progress[0].TotalFilesRemaining.ToString();
                Pl1TotalFilesToCopy = progress[0].TotalFilesToCopy.ToString();
                
            }
            if (progress[1] != null)
            {
                Pl2Progression = progress[1].Progression.ToString();
                Pl2Name = progress[1].Name;
                Pl2SizeRemaining = progress[1].SizeRemainingFormatted.ToString();
                Pl2State = progress[1].State;
                Pl2TotalFilesRemaining = progress[1].TotalFilesRemaining.ToString();
                Pl2TotalFilesToCopy = progress[1].TotalFilesToCopy.ToString();
            }
            if (progress[2] != null)
            {
                Pl3Progression = progress[2].Progression.ToString();
                Pl3Name = progress[2].Name;
                Pl3SizeRemaining = progress[2].SizeRemainingFormatted.ToString();
                Pl3State = progress[2].State;
                Pl3TotalFilesRemaining = progress[2].TotalFilesRemaining.ToString();
                Pl3TotalFilesToCopy = progress[2].TotalFilesToCopy.ToString();
            }
            if (progress[3] != null)
            {
                Pl4Progression = progress[3].Progression.ToString();
                Pl4Name = progress[3].Name;
                Pl4SizeRemaining = progress[3].SizeRemainingFormatted.ToString();
                Pl4State = progress[3].State;
                Pl4TotalFilesRemaining = progress[3].TotalFilesRemaining.ToString();
                Pl4TotalFilesToCopy = progress[3].TotalFilesToCopy.ToString();
            }
            if (progress[4] != null)
            {
                Pl5Progression = progress[4].Progression.ToString();
                Pl5Name = progress[4].Name;
                Pl5SizeRemaining = progress[4].SizeRemainingFormatted.ToString();
                Pl5State = progress[4].State;
                Pl5TotalFilesRemaining = progress[4].TotalFilesRemaining.ToString();
                Pl5TotalFilesToCopy = progress[4].TotalFilesToCopy.ToString();
            }
        }

        /// <summary>
        /// Send the state of the progress 
        /// </summary>
        public void SendInformation(String txt)
        {

            //Send the list to the client
            try
            {
                server.Send(Encoding.UTF8.GetBytes(txt));
            }
            catch (SocketException)
            {
            }

        }
    }
}