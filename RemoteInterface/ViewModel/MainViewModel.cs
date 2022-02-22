using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace RemoteInterface
{
    delegate void Del(JobBackup jobBackup);

    class MainViewModel : INotifyPropertyChanged
    {
        //Attributes
        private ObservableCollection<JobBackup> _listOfJobBackup = null;
        private Communication _communication;
        private int _selectedIndex;
        private JobBackup _job;
        private double _totalFilesSizeFormatted;
        private string _jobTypeFormatted;
        //Define getter / setter
        public ObservableCollection<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set { _listOfJobBackup = value; OnPropertyChanged("ListOfJobBackup"); } }
        public JobBackup Job
        {
            get => _job;
            set
            {
                _job = value;
                OnPropertyChanged("Job");
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

        /// <summary>
        /// Constructor of MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            //No job backup selected
            SelectedIndex = -1;
        }
        //Instanciate the delegate
        readonly Del Execute = delegate (JobBackup jobBackup)
        {
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public void CommunicationWithRemoteInterface()
        {
            Communication comm = new Communication();
            comm.LaunchConnection();
        }


        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Resume threads or instanciate a thread and execute the jobBackup in this thread.
        /// </summary>
        /// <param name="jobBackup">The JobBackup to execute.</param>
        public void ExecuteOne(JobBackup jobBackup)
        {
            Communication comm = new Communication();
        }


        /// <summary>
        /// Execute all the list of JobBackup with the ExecuteOne method.
        /// </summary>
        /// <remarks>Threads are executed one by one, in the order of the list.</remarks>
        public void ExecuteAll(List<JobBackup> jbList)
        {
            Communication comm = new Communication();
        }

        public void StopConnection()
        {
            Communication comm = new Communication();
            comm.SendStopConnection();
        }

        /// <summary>
        /// Pause all JobBackups threads.
        /// </summary>
        public void Pause()
        {
        }


        /// <summary>
        /// Stop all JobBackups threads.
        /// </summary>
        public void Stop()
        {
        }
    }
}