using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace EasySave.ViewModel
{
    class MainViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;
        private JobBackup _jobBackup;
        private JsonReadWriteModel _jsonReadWriteModel;

        ///  <summary>Constructor of MainViewModel.</summary>
        public MainViewModel()
        {
            //Read the list in the json
            _jsonReadWriteModel = new JsonReadWriteModel();
            _listOfJobBackup = _jsonReadWriteModel.ReadJobBackup();
        }
        public void DeleteSave(int id)
        {
            JsonReadWriteModel JSonReaderWriter = new JsonReadWriteModel();
            int count = 0;
            //Search in the list if there's an id similar to the selected one
            foreach (JobBackup item in _listOfJobBackup)
            {
                if (count == id)
                {
                    //check if the list isn't empty
                    if (count == 0)
                    {
                        _listOfJobBackup.Remove(item);
                    }
                    else
                    {
                        //Update the index of the elements in the list
                        _listOfJobBackup[count].Id = _listOfJobBackup.Count - 1;
                        //Remove the Job Backup from the list
                        _listOfJobBackup.Remove(item);
                    }

                    //Save the list in json
                    JSonReaderWriter.SaveJobBackup(_listOfJobBackup);
                    break;
                }
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
                item.Execute(Configuration.GetInstance().BusinessSoftware);
            }
        }
        public List<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set => _listOfJobBackup = value; }
    }
}
