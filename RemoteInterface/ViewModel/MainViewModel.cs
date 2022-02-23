using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RemoteInterface
{
    delegate void Del(JobBackup jobBackup);

    class MainViewModel : INotifyPropertyChanged
    {
        //Attributes
        private ObservableCollection<ProgressLog> _listOfProgressLog = null;
        private ObservableCollection<JobBackup> _listOfJobBackup = null;
        private Task _mainThread = null;
        private Task _thread1 = null;
        private Task _thread2 = null;
        private int _selectedIndex;
        private JobBackup _job;
        private ProgressLog _progressLog;
        private double _totalFilesSizeFormatted;
        private string _jobTypeFormatted;

        //Define getter / setter
        public ObservableCollection<ProgressLog> ListOfProgressLog { get => _listOfProgressLog; set { _listOfProgressLog = value; OnPropertyChanged("ListOfJobBackup"); } }

        //Define getter / setter
        public ObservableCollection<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set { _listOfJobBackup = value; OnPropertyChanged("ListOfJobBackup"); } }
        public ProgressLog PLog
        {
            get => _progressLog;
            set
            {
                _progressLog = value;
                OnPropertyChanged("ProgressLog");
            }
        }

        public double TotalFilesSizeFormatted
        {
            get
            {
                //Convert in MO
                return (double)Math.Round(_totalFilesSizeFormatted / 1048576, 2);
            }
            set
            {
                _totalFilesSizeFormatted = value;
                OnPropertyChanged("TotalFilesSizeFormatted");
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public string JobTypeFormatted
        {
            get
            {
                return _jobTypeFormatted;
            }
            set
            {
                _jobTypeFormatted = value;
                OnPropertyChanged("JobTypeFormatted");
            }
        }

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
            //No job backup selected
            SelectedIndex = -1;
            Communication.LaunchConnection();

        }
        //Instanciate the delegate
        readonly Del Execute = delegate (JobBackup jobBackup)
        {
        };


        /*public void CommunicationWithRemoteInterface()
        {
            Communication comm = new Communication();
            comm.LaunchConnection();
        }*/

        /*public void receiveContinuously()
        {
            int count = 0;
            if (_listOfProgressLog == null ||_listOfProgressLog.Count < 4)
            {
                _mainThread = Task.Run(() =>
                {
                    Communication comm = new Communication();
                    _thread1= Task.Run(() =>
                    {
                    while (comm.Connected)
                    {
                        _listOfProgressLog.Add(comm.receiveProgessLog());
                    }
                    });
                });
            }
            else
            {
                _mainThread = Task.Run(() =>
                {
                    Communication comm = new Communication();
                    while (comm.Connected)
                    {
                        _listOfProgressLog[count]= comm.receiveProgessLog();
                        count++;
                        if (count >4)
                        {
                            count = 0;       
                        }
                    }
                });
            }
        }*/


        /// <summary>
        /// Resume threads or instanciate a thread and execute the jobBackup in this thread.
        /// </summary>
        /// <param name="jobBackup">The JobBackup to execute.</param>
        public void ExecuteOne(JobBackup jobBackup)
        {
            //Communication comm = new Communication();
        }


        /// <summary>
        /// Execute all the list of JobBackup with the ExecuteOne method.
        /// </summary>
        /// <remarks>Threads are executed one by one, in the order of the list.</remarks>
        public void ExecuteAll(List<JobBackup> jbList)
        {
            //Communication comm = new Communication();
        }

  

        /// <summary>
        /// Pause all JobBackups threads.
        /// </summary>
        public void Pause()
        {
            /*Communication comm = new Communication();
            comm.SendInformation("Stop");*/
        }


        /// <summary>
        /// Stop all JobBackups threads.
        /// </summary>
        public void Stop()
        {
            /*Communication comm = new Communication();
            comm.SendInformation("Reset");*/
        }
    }
}