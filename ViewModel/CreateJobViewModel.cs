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
            MessageBox.Show(_jobBackup.Id.ToString());
            //Set value in combobox according to the type of backup
            if (_jobBackup.IsDifferential) _selectedIndex = 1;
            if (!_jobBackup.IsDifferential) _selectedIndex = 0;
        }

        public JobBackup JobBackup { get => _jobBackup; set => _jobBackup = value; }
        public int SelectedIndex { get => _selectedIndex; set => _selectedIndex = value; }

        ///  <summary>Create the job</summary>
        public void JobCreation(string label,string SourceDirectory, string DestinationDirectory, bool isDifferential)
        {
            MainViewModel main = new MainViewModel();
            List<JobBackup> list = main.ListOfJobBackup;
            MessageBox.Show(this._jobBackup.Id.ToString());
            JsonReadWriteModel JSonReaderWriter = new JsonReadWriteModel();
            list.Add(new JobBackup(list.Count - 1));

            list[list.Count - 1].Label = label;
            list[list.Count - 1].SourceDirectory = SourceDirectory;
            list[list.Count - 1].DestinationDirectory = DestinationDirectory;
            list[list.Count - 1].IsDifferential = isDifferential;

            //Ajoute un élément dans la liste
            //Save the JobBackup list in JSON file
            JSonReaderWriter.SaveJobBackup(list);
            //_view.Display(String.Format(_rm.GetString("jobBackupSuccessCreated"), _listOfJobBackup[input].Label) + Environment.NewLine);
            //_view.Display("  " + _listOfJobBackup[input].Label + _rm.GetString("jobBackupSuccessCreated"));
        }

    }
}
