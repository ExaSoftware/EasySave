using System.Collections.Generic;
using System.Reflection;

namespace EasySave.ViewModel
{
    class MainViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;
        private JsonReadWriteModel _jsonReadWriteModel;

        ///  <summary>Constructor of MainViewModel.</summary>
        public MainViewModel()
        {
            //Creation of a list of 5 JobBackup
            //_listOfJobBackup = Init.CreateJobBackupList();
            _jsonReadWriteModel = new JsonReadWriteModel();
            _listOfJobBackup = _jsonReadWriteModel.ReadJobBackup();
        }
        public void DeleteSave()
        {
            JsonReadWriteModel JSonReaderWriter = new JsonReadWriteModel();
            int count = 0;
            //Delete element from list
            foreach (JobBackup item in _listOfJobBackup)
            {
                //Check the ID selected and delete the corresponding ID in the list / Json
                //if (count == input)
                //{
                //   _listOfJobBackup.Remove(item);
                //   JSonReaderWriter.SaveJobBackup(_listOfJobBackup);
                //   break;
                //}
                count++;
            }
        }

        public void ExecuteOne(JobBackup jobBackup)
        {
            jobBackup.Execute(Configuration.GetInstance().BusinessSoftware);
        }
        public void ExecuteAll()
        {
            foreach (JobBackup item in _listOfJobBackup)
            {
                //item.Execute(new List<string>());
            }
        }
        public List<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set => _listOfJobBackup = value; }
    }
}
