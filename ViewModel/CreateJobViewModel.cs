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
        public CreateJobViewModel()
        {
            
        }

        public CreateJobViewModel(JobBackup jobBackup)
        {
            _jobBackup = jobBackup;

            if (_jobBackup.IsDifferential) _selectedIndex = 1;
            if (!_jobBackup.IsDifferential) _selectedIndex = 0;
        }

        public JobBackup JobBackup { get => _jobBackup; set => _jobBackup = value; }
        public int SelectedIndex { get => _selectedIndex; set => _selectedIndex = value; }

        ///  <summary>Create the job</summary>
        public void JobCreation(string label,string SourceDirectory, string DestinationDirectory, bool isDifferential)
        {
            MainViewModel detail = new MainViewModel();
            List<JobBackup> list = detail.ListOfJobBackup;
            JsonReadWriteModel JSonReaderWriter = new JsonReadWriteModel();

            list[list.Count].Label = label;
            list[list.Count].SourceDirectory = SourceDirectory;
            list[list.Count].DestinationDirectory = DestinationDirectory;
            list[list.Count].IsDifferential = isDifferential;

            //Ajoute un élément dans la liste
            list.Add(new JobBackup(list.Count));
            //Save the JobBackup list in JSON file
            JSonReaderWriter.SaveJobBackup(list);

            Console.Clear();
            //_view.Display(String.Format(_rm.GetString("jobBackupSuccessCreated"), _listOfJobBackup[input].Label) + Environment.NewLine);
            //_view.Display("  " + _listOfJobBackup[input].Label + _rm.GetString("jobBackupSuccessCreated"));
        }

    }
}
