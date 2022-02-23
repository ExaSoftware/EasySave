using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Resources;

namespace EasySave.ViewModel
{
    public class CreateJobViewModel
    {
        private JobBackup _jobBackup;
        private ObservableCollection<JobBackup> _jbList;
        private int _selectedIndex;
        private int _prioritySelectedIndex;
        private JsonReadWriteModel _jsonReadWriteModel = new JsonReadWriteModel();

        //RessourceManager for the strings translation
        private ResourceManager _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
        //Dictionary which contain error associate to a field
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        /// <summary>
        /// Constructor used when we want to add a new jobackup
        /// </summary>
        public CreateJobViewModel(ObservableCollection<JobBackup> list)
        {
            //When we add a job backup, set the default combobox value
            _selectedIndex = 0;
            _jbList = list;
        }

        /// <summary>
        /// Constructor used to bind the view with selected job backup in the list
        /// </summary>
        /// <param name="jobBackup"></param>
        public CreateJobViewModel(JobBackup jobBackup, ObservableCollection<JobBackup> list)
        {
            _jobBackup = jobBackup;
            _jbList = list;
            //Set value in combobox according to the type of backup
            if (_jobBackup.IsDifferential) _selectedIndex = 1;
            if (!_jobBackup.IsDifferential) _selectedIndex = 0;

            //Set value in combobox according to the priority
            _prioritySelectedIndex = _jobBackup.Priority;
        }

        public JobBackup JobBackup { get => _jobBackup; set => _jobBackup = value; }
        public int SelectedIndex { get => _selectedIndex; set => _selectedIndex = value; }
        public Dictionary<string, string> Errors { get => _errors; set => _errors = value; }
        public int PrioritySelectedIndex { get => _prioritySelectedIndex; set => _prioritySelectedIndex = value; }

        ///  <summary>Create or modifiy the job</summary>
        public void JobCreation(int id, string label, string sourceDirectory, string destinationDirectory, bool isDifferential, int priority)
        {

            //Modify a list
            if (id != -1)
            {
                //Ajoute un élément dans la liste
                _jbList[id].Fill(label, sourceDirectory, destinationDirectory, isDifferential, priority);
            }
            //Insert a new list if it's not modified
            else
            {
                //Ajoute un élément dans la liste


                JobBackup newJobBackup = new JobBackup();
                newJobBackup.Fill(label, sourceDirectory, destinationDirectory, isDifferential, priority);
                newJobBackup.Id = _jbList.Count;
                _jbList.Add(newJobBackup);
            }
            //Save the JobBackup list in JSON file
            _jsonReadWriteModel.SaveJobBackup(_jbList);
        }

        /// <summary>
        /// Check Label method. Add an error in the dictionnary if necessary
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public void CheckLabel(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                _errors.Add("labelError", _rm.GetString("emptyLabelError"));
            }
            else
            {
                if (text.Length > 30) _errors.Add("labelError", _rm.GetString("lengthLabelError"));
            }
        }

        /// <summary>
        /// Check Source directory method. Add an error in the dictionnary if necessary
        /// </summary>
        /// <param name="sourcePath">The source path to check</param>
        public void CheckSourceDirectory(string sourcePath)
        {
            //We need to check if directory exists
            if (String.IsNullOrEmpty(sourcePath))
            {
                _errors.Add("sourceDirectoryError", _rm.GetString("emptySourceDirectoryError"));
            }
            else
            {   //Check if directory exists
                if (!Directory.Exists(sourcePath))
                {
                    _errors.Add("sourceDirectoryError", _rm.GetString("sourceDirectoryNotExistsError"));
                }
            }

        }

        /// <summary>
        /// Check Source directory method. Add an error in the dictionnary if necessary
        /// </summary>
        /// <param name="destinationPath">The destination path to check</param>
        /// <param name="sourcePath">The source path previously filled</param>
        public void CheckDestinationDirectory(string destinationPath, string sourcePath)
        {
            if (String.IsNullOrEmpty(destinationPath))
            {
                _errors.Add("destinationDirectoryError", _rm.GetString("emptyDestinationDirectoryError"));
            }
            else
            {
                if (destinationPath.Equals(sourcePath)) _errors.Add("destinationDirectoryError", _rm.GetString("destinationPathEqualsSourcePath"));
            }
        }
    }
}
