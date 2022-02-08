using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.ViewModel
{
    class CreateJobViewModel
    {
        public CreateJobViewModel()
        {
        }

        ///  <summary>Create the job</summary>
        public void JobCreation(string label,string SourceDirectory, string DestinationDirectory, bool isDifferential)
        {
            DetailViewModel detail = new DetailViewModel();
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
