using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace EasySave.ViewModel
{
    class DetailViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;
        private JsonReadWriteModel _jsonReadWriteModel;

        ///  <summary>Constructor of MainViewModel.</summary>
        public DetailViewModel()
        {
            //Creation of a list of 5 JobBackup
            //_listOfJobBackup = Init.CreateJobBackupList();
            _jsonReadWriteModel = new JsonReadWriteModel();
            _listOfJobBackup = _jsonReadWriteModel.ReadJobBackup();
        }
        public void Start()
        {
            JsonReadWriteModel JSonReaderWriter = new JsonReadWriteModel();
            //Check the number of jobBackup in the list and update the count

        }
        public List<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set => _listOfJobBackup = value; }
    }
}
