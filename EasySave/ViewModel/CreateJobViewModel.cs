using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;

namespace EasySave.ViewModel
{
    public class CreateJobViewModel
    {
        private JobBackup _jobBackup;
        private int _selectedIndex;

        //RessourceManager for the strings translation
        private ResourceManager _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
        //Dictionary which contain error associate to a field
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

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
        public Dictionary<string, string> Errors { get => _errors; set => _errors = value; }

        ///  <summary>Create or modifiy the job</summary>
        public void JobCreation(int id, string label, string sourceDirectory, string destinationDirectory, bool isDifferential)
        {
            MainViewModel main = new MainViewModel();
            List<JobBackup> list = main.ListOfJobBackup;

            //Modify a list
            if (id != -1)
            {
                //Ajoute un élément dans la liste
                list[id].Fill(label, sourceDirectory, destinationDirectory, isDifferential);
            }
            //Insert a new list if it's not modified
            else
            {
                //Ajoute un élément dans la liste

                JobBackup newJobBackup = new JobBackup();
                newJobBackup.Fill(label, sourceDirectory, destinationDirectory, isDifferential);
                newJobBackup.Id = list.Count;
                list.Add(newJobBackup);
            }
            //Save the JobBackup list in JSON file
            JsonReadWriteModel.SaveJobBackup(list);
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
