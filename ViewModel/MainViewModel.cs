﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace EasySave.ViewModel
{
    delegate void Del(JobBackup jobBackup);

    class MainViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;
        private JsonReadWriteModel _jsonReadWriteModel;

        //Define getter / setter
        public List<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set => _listOfJobBackup = value; }

        /// <summary>
        /// Constructor of MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            //Read the list in the json
            _jsonReadWriteModel = new JsonReadWriteModel();
            _listOfJobBackup = _jsonReadWriteModel.ReadJobBackup();
        }

        /// <summary>
        /// Delete the selected save in the list of JobBackup
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSave(int id)
        {
            JsonReadWriteModel JSonReaderWriter = new JsonReadWriteModel();
            int count = 0;
            if (_listOfJobBackup.Count !=0) { 
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
            JSonReaderWriter.SaveJobBackup(_listOfJobBackup);
        }

        //Instanciate the delegate
        readonly Del Execute = delegate (JobBackup jobBackup)
        {
            jobBackup.Execute(App.Configuration.BusinessSoftware);
            Trace.WriteLine(App.Configuration.BusinessSoftware);
        };

        /// <summary>
        /// Instanciate a thread and execute the jobBackup in this thread.
        /// </summary>
        /// <param name="jobBackup"></param>
        public void ExecuteOne(JobBackup jobBackup)
        {
            Thread thread = new Thread(() => Execute(jobBackup));
            thread.Start();

        }

        /// <summary>
        /// Execute all the list with ExecuteOne method.
        /// </summary>
        public void ExecuteAll()
        {
            foreach (JobBackup item in _listOfJobBackup)
            {
                ExecuteOne(item);
            }
        }

    }
}
