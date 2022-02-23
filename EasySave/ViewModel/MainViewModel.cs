using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EasySave.ViewModel
{
    delegate void Del(JobBackup jobBackup);

    class MainViewModel : INotifyPropertyChanged
    {
        //Attributes
        private ObservableCollection<JobBackup> _listOfJobBackup = null;
        private Task _thread1 = null;
        private Task _thread2 = null;
        private Task _thread3 = null;
        private Task _thread4 = null;
        private Task _thread5 = null;
        private Task _mainThread = null;
        private int _selectedIndex;
        private JobBackup _job;
        private double _totalFilesSizeFormatted;
        private string _jobTypeFormatted;

        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        private JsonReadWriteModel _jsonReadWriteModel = new JsonReadWriteModel();


        private List<JobBackup> _veryHightPriority = new List<JobBackup>();
        private List<JobBackup> _hightPriority = new List<JobBackup>();
        private List<JobBackup> _normalPriority = new List<JobBackup>();

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
            //Read the list in the json
            
            _listOfJobBackup = _jsonReadWriteModel.ReadJobBackup();

            //No job backup selected
            SelectedIndex = -1;

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

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
                            if(Directory.Exists(@"C:\EasySave\Logs\")) File.Delete(@"C:\EasySave\Logs\ProgressLog.json");
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
                            _jsonReadWriteModel.DeleteProgressLogInJson(item.Label);

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
            _jsonReadWriteModel.SaveJobBackup(_listOfJobBackup);
            
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

        private delegate void DelJbMv(JobBackup jobBackup, MainViewModel mainViewModel);

        private DelJbMv GetTotalFileSize = delegate (JobBackup jobBackup, MainViewModel mainViewModel)
        {
            long result = 0;
            mainViewModel.TotalFilesSizeFormatted = result;

            result = jobBackup.TotalFileSize();

            mainViewModel.TotalFilesSizeFormatted = result;
        };

        /// <summary>
        /// Actualise the TotalFileSize field
        /// </summary>
        /// <param name="jobBackup">The job to get the total file size.</param>
        public void SetTotalFileSize(JobBackup jobBackup)
        {
            //Get the totalFileSize from the VM
            _ = Task.Run(() => GetTotalFileSize(jobBackup, this));

        }

        /// <summary>
        /// Sort a list of jobBackup by priority
        /// </summary>
        /// <param name="listOfJobBackup"></param>
        public void SortList(List<JobBackup> listOfJobBackup)
        {
            foreach ( JobBackup jobBackup in listOfJobBackup)
            {
                if (jobBackup.Priority == 0) _veryHightPriority.Add(jobBackup);
                else if (jobBackup.Priority == 1) _hightPriority.Add(jobBackup);
                else _normalPriority.Add(jobBackup);
            }
            Task.Run(() =>
            {
                Task.Run(() => ExecuteAll(_veryHightPriority)).Wait();
                Task.Run(() => ExecuteAll(_hightPriority)).Wait();
                Task.Run(() => ExecuteAll(_normalPriority)).Wait();
            });
        }

        /// <summary>
        /// Execute all the list of JobBackup with the ExecuteOne method.
        /// </summary>
        /// <remarks>Threads are executed one by one, in the order of the list.</remarks>
        private void ExecuteAll(List<JobBackup> jbList)
        {
            App.ThreadPause = false;

            if (_mainThread is null || _mainThread.IsCompleted)
            {
                App.ThreadPause = false;

                _mainThread = Task.Run(() =>
                {
                    Task[] taskArray = new Task[]{ _thread1, _thread2, _thread3, _thread4, _thread5 };
                    double numberOfIteration = Math.Round((double)(jbList.Count / 5));
                    int i = 0;

                    for (i = 0; i < numberOfIteration * 5; i += 5)
                    {
                        _thread1 = Task.Run(() => Execute(jbList[i]));
                        _thread2 = Task.Run(() => Execute(jbList[i++]));
                        _thread3 = Task.Run(() => Execute(jbList[i + 2]));
                        _thread4 = Task.Run(() => Execute(jbList[i + 3]));
                        _thread5 = Task.Run(() => Execute(jbList[i + 4]));

                        _thread1.Wait();
                        _thread2.Wait();
                        _thread3.Wait();
                        _thread4.Wait();
                        _thread5.Wait();

                        GC.Collect();
                    }
                    
                    for (int a = 0; a < jbList.Count - (int)numberOfIteration; a++)
                    {
                        int b = a;
                        taskArray[b % 5] = Task.Run(() => Execute(jbList[b]));
                    }

                    foreach (Task task in taskArray)
                    {
                        if (!(task is null))
                        {
                            try
                            {
                                task.Wait();
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    GC.Collect();
                });

            }
        }

        /// <summary>
        /// Pause all JobBackups threads.
        /// </summary>
        public void Pause()
        {
            App.ThreadPause = true;
        }

        /// <summary>
        /// Stop all JobBackups threads.
        /// </summary>
        public void Stop()
        {
            if (!(_mainThread is null))
            {
                
            }
        }

    }
}