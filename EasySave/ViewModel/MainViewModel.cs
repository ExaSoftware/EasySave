using EasySave.Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EasySave.ViewModel
{

    class MainViewModel : INotifyPropertyChanged
    {
        //Delegate
        private delegate void Del(JobBackup jobBackup, CancellationToken token);
        private delegate void DelegateComm(List<JobBackup> jobBackupsList, int u);

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


        private List<JobBackup> _veryHightPriority = null;
        private List<JobBackup> _hightPriority = null;
        private List<JobBackup> _normalPriority = null;

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
                            if (Directory.Exists(@"C:\EasySave\Logs\")) File.Delete(@"C:\EasySave\Logs\ProgressLog.json");
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
        private readonly Del Execute = delegate (JobBackup jobBackup, CancellationToken token)
        {
            if (App.Configuration.BusinessSoftware != "" || App.Configuration.BusinessSoftware != null)
            {
                Process[] procName = Process.GetProcessesByName(App.Configuration.BusinessSoftware);
                if (procName.Length == 0)
                {
                    jobBackup.Execute(token);
                }
            }
            else
            {
                jobBackup.Execute(token);
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
            _veryHightPriority = new List<JobBackup>();
            _hightPriority = new List<JobBackup>();
            _normalPriority = new List<JobBackup>();

            //if(_mainThread.Status.Equals(TaskStatus.Running))

            unPause();

            if (_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Dispose();
            }

            foreach (JobBackup jobBackup in listOfJobBackup)
            {
                if (jobBackup.Priority == 0) _veryHightPriority.Add(jobBackup);
                else if (jobBackup.Priority == 1) _hightPriority.Add(jobBackup);
                else _normalPriority.Add(jobBackup);
            }
            _mainThread = Task.Run(() =>
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
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            Task[] taskArray = new Task[] { _thread1, _thread2, _thread3, _thread4, _thread5 };

            double numberOfIteration = Math.Round((double)(jbList.Count / 5));
            int i = 0;

            //Execute 5 by 5
            for (i = 0; i < numberOfIteration * 5; i += 5)
            {
                _thread1 = Task.Run(() => Execute(jbList[i], _token));
                _thread2 = Task.Run(() => Execute(jbList[i++], _token));
                _thread3 = Task.Run(() => Execute(jbList[i + 2], _token));
                _thread4 = Task.Run(() => Execute(jbList[i + 3], _token));
                _thread5 = Task.Run(() => Execute(jbList[i + 4], _token));

                CancellationTokenSource remoteTokenSource = new CancellationTokenSource();
                CancellationToken remoteToken = remoteTokenSource.Token;

                Task.Run(() =>
                {
                    while (true)
                    {
                        ProgressLog[] progressArray = new ProgressLog[5];
                        try
                        {
                            remoteToken.ThrowIfCancellationRequested();

                            progressArray[0] = !(jbList[i] is null) ? jbList[i].State : progressArray[0];
                            progressArray[1] = !(jbList[i++] is null) ? jbList[i++].State : progressArray[1];
                            progressArray[2] = !(jbList[i + 2] is null) ? jbList[i + 2].State : progressArray[2];
                            progressArray[3] = !(jbList[i + 3] is null) ? jbList[i + 3].State : progressArray[3];
                            progressArray[4] = !(jbList[i + 4] is null) ? jbList[i + 4].State : progressArray[4];

                            //Send progressArray
                            Thread.Sleep(5000);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                    }
                });

                _thread1.Wait();
                _thread2.Wait();
                _thread3.Wait();
                _thread4.Wait();
                _thread5.Wait();

                remoteTokenSource.Cancel();
                remoteTokenSource.Dispose();
                GC.Collect();
            }

            //Execute other tasks
            for (int a = (int)numberOfIteration; a < jbList.Count; a++)
            {
                int b = a;
                taskArray[b] = Task.Run(() => Execute(jbList[b], _token));
            }

            CancellationTokenSource remoteTokenSource2 = new CancellationTokenSource();
            CancellationToken remoteToken2 = remoteTokenSource2.Token;

            Task.Run(() =>
            {
                while (true)
                {
                    ProgressLog[] progressArray = new ProgressLog[5];
                    try
                    {
                        remoteToken2.ThrowIfCancellationRequested();

                        for (int a = (int)numberOfIteration; a < jbList.Count; a++)
                        {
                            int b = a;
                            progressArray[b % 5] = !(jbList[b] is null) ? jbList[b].State : progressArray[0];
                        }

                        //Send progressArray
                        Thread.Sleep(5000);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            });

            foreach (Task task in taskArray)
            {
                if (!(task is null))
                {
                    task.Wait();
                }
            }

            remoteTokenSource2.Cancel();
            remoteTokenSource2.Dispose();
            GC.Collect();
        }

        /// <summary>
        /// Unpause all JobBackups threads.
        /// </summary>
        public void unPause()
        {
            App.ThreadPause = false;
        }

        /// <summary>
        /// Pause all JobBackups threads.
        /// </summary>
        public void Pause()
        {
            App.ThreadPause = App.ThreadPause ? false : true;
        }

        /// <summary>
        /// Stop all JobBackups threads.
        /// </summary>
        public void Stop()
        {
            if (!(_mainThread is null) && _mainThread.Status.Equals(TaskStatus.Running))
            {
                _tokenSource.Cancel();
            }
        }

    }
}