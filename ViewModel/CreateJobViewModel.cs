using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EasySave.ViewModel
{
    public class CreateJobViewModel
    {
        private JobBackup _jobBackup;
        private int _selectedIndex;

        /// <summary>
        /// Constructor used when we want to add a new jobackup
        /// </summary>
        public CreateJobViewModel()
        {
            //When we add a job backup, set the default combobox value
            _selectedIndex = 0;
        }

        /// <summary>
        /// Constructor used to bind the view with selected job backup in the list
        /// </summary>
        /// <param name="jobBackup"></param>
        public CreateJobViewModel(JobBackup jobBackup)
        {
            _jobBackup = jobBackup;
            //Set value in combobox according to the type of backup
            if (_jobBackup.IsDifferential) _selectedIndex = 1;
            if (!_jobBackup.IsDifferential) _selectedIndex = 0;
        }

        public JobBackup JobBackup { get => _jobBackup; set => _jobBackup = value; }
        public int SelectedIndex { get => _selectedIndex; set => _selectedIndex = value; }

        ///  <summary>Create or modifiy the job</summary>
        public void JobCreation(int id, string label, string sourceDirectory, string destinationDirectory, bool isDifferential)
        {
            MainViewModel main = new MainViewModel();
            List<JobBackup> list = main.ListOfJobBackup;
            JsonReadWriteModel JSonReaderWriter = new JsonReadWriteModel();

            //Modify a list
            if (id != -1)
            {
                //Ajoute un élément dans la liste
                list[id].Label = label;
                list[id].SourceDirectory = sourceDirectory;
                list[id].DestinationDirectory = destinationDirectory;
                list[id].IsDifferential = isDifferential;
            }
            //Insert a new list if it's not modified
            else
            {
                //Ajoute un élément dans la liste
                list.Add(new JobBackup());
                list[list.Count-1].Label = label;
                list[list.Count-1].SourceDirectory = sourceDirectory;
                list[list.Count-1].DestinationDirectory = destinationDirectory;
                list[list.Count-1].IsDifferential = isDifferential;
                list[list.Count-1].Id = list.Count - 1;
            }
            //Save the JobBackup list in JSON file
            JSonReaderWriter.SaveJobBackup(list);
        }

    }
}
