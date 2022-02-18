using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace EasySave.ViewModel
{
    delegate void Del(JobBackup jobBackup);

    class MainViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;
        private Thread _thread;
        private Thread _sequentialThread;

        //Define getter / setter
        public List<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set => _listOfJobBackup = value; }

        /// <summary>
        /// Constructor of MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            //Read the list in the json
            _listOfJobBackup = JsonReadWriteModel.ReadJobBackup();
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
                            _listOfJobBackup.Remove(item);
                            _listOfJobBackup.Clear();
                            item.Dispose();
                        }
                        else
                        {
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

        /// <summary>
        /// Instanciate a thread and execute the jobBackup in this thread.
        /// </summary>
        /// <param name="jobBackup">The JobBackup to execute.</param>
        public void ExecuteOne(JobBackup jobBackup)
        {
            _thread = new Thread(() => Execute(jobBackup));
            _thread.Start();
        }

        /// <summary>
        /// Execute all the list of JobBackup with the ExecuteOne method.
        /// </summary>
        /// <remarks>Threads are executed one by one, in the order of the list.</remarks>
        public void ExecuteAll()
        {
            _sequentialThread = new Thread(() =>
            {
                foreach (JobBackup item in _listOfJobBackup)
                {
                    ExecuteOne(item);
                    _thread.Join();
                }
            });
            _sequentialThread.Start();

        }

    }
}