using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;

namespace EasySave.ViewModel
{
    delegate void Del(JobBackup jobBackup);

    class MainViewModel : INotifyPropertyChanged
    {
        //Attributes
        private ObservableCollection<JobBackup> _listOfJobBackup = null;
        private Communication _communication;
        private Thread _thread = null;
        private Thread _thread1 = null;
        private Thread _thread2 = null;
        private Thread _thread3 = null;
        private Thread _thread4 = null;
        private Thread _thread5 = null;
        private Thread _mainThread = null;
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
            //Read the list in the json
            _listOfJobBackup = JsonReadWriteModel.ReadJobBackup();

            //No job backup selected
            SelectedIndex = -1;

            CommunicationWithRemoteInterface();


        }

        /// <summary>
        /// Delete the selected save in the list of JobBackup
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSave(int id)
        {
            int count = 0;
            if (_listOfJobBackup.Count != 0)
            {
                //Search in the list if there's an id similar to the selected one
                foreach (JobBackup item in _listOfJobBackup)
                {
                    if (count == id)
                    {
                        //check if the list isn't empty
                        if (_listOfJobBackup.Count == 1)
                        {
                            //Remove the progressLog in json link to this jobBackup
                            File.Delete(@"C:\EasySave\Logs\ProgressLog.json");
                            _listOfJobBackup.Remove(item);
                            _listOfJobBackup.Clear();
                            item.Dispose();
                            Job = null;
                            TotalFilesSizeFormatted = 0;
                            JobTypeFormatted = String.Empty;
                        }
                        else
                        {
                            //Remove the progressLog in json link to this jobBackup
                            JsonReadWriteModel.DeleteProgressLogInJson(item.Label);

                            //Remove the Job Backup from the list
                            _listOfJobBackup.Remove(item);
                            item.Dispose();
                            //Update the index of the elements in the list
                        }
                        break;

                    }
                    count++;
                }
                if (_listOfJobBackup.Count != 0)
                {
                    for (count = 0; count < _listOfJobBackup.Count; count++)
                    {
                        _listOfJobBackup[count].Id = count;
                    }
                }

            }
            //Save the list in json
            JsonReadWriteModel.SaveJobBackup(_listOfJobBackup);
            
        }

        //Instanciate the delegate
        readonly Del Execute = delegate (JobBackup jobBackup)
        {
            if (App.Configuration.BusinessSoftware != "" || App.Configuration.BusinessSoftware != null)
            {
                Process[] procName = Process.GetProcessesByName(App.Configuration.BusinessSoftware);
                if (procName.Length == 0)
                {
                    jobBackup.Execute();
                }
            }
            else
            {
                jobBackup.Execute();
            }
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

            Job = jobBackup;
            SelectedIndex = Job.Id;
            _mainThread = new Thread(() =>
            {
                _thread = new Thread(() => Execute(jobBackup));
                _thread.Start();
            });
            _mainThread.Start();
        }


        /// <summary>
        /// Execute all the list of JobBackup with the ExecuteOne method.
        /// </summary>
        /// <remarks>Threads are executed one by one, in the order of the list.</remarks>
        public void ExecuteAll(List<JobBackup> jbList)
        {
            if (_mainThread is null || !_mainThread.IsAlive)
            {
                _mainThread = new Thread(() =>
                {
                    List<Thread> threadList = new List<Thread>() { _thread1, _thread2, _thread3, _thread4, _thread5 };
                    double numberOfIteration = Math.Round((double)(jbList.Count / 5));
                    int i = 0;

                    for (i = 0; i < numberOfIteration * 5; i += 5)
                    {
                        _thread1 = new Thread(() => Execute(jbList[i]));
                        _thread2 = new Thread(() => Execute(jbList[i++]));
                        _thread3 = new Thread(() => Execute(jbList[i + 2]));
                        _thread4 = new Thread(() => Execute(jbList[i + 3]));
                        _thread5 = new Thread(() => Execute(jbList[i + 4]));

                        _thread1.Start();
                        _thread2.Start();
                        _thread3.Start();
                        _thread4.Start();
                        _thread5.Start();

                        _thread1.Join();
                        _thread2.Join();
                        _thread3.Join();
                        _thread4.Join();
                        _thread5.Join();
                    }

                    for (int a = 0; a < jbList.Count; a++)
                    {
                        int b = a;
                        threadList[b % 5] = new Thread(() => Execute(jbList[b]));
                        threadList[b % 5].Start();
                    }

                    foreach (Thread thread in threadList)
                    {
                        if (!(thread is null))
                        {
                            thread.Join();
                        }
                    }

                });
                _mainThread.Start();
            }
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
            try
            {
                _mainThread.Abort();
            }
            catch
            {

            }
        }

    }
}