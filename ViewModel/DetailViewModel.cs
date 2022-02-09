﻿using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace EasySave.ViewModel
{
    class DetailViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;
        private ResourceManager _rm;
        private View _view;
        private Configuration _configuration;

        ///  <summary>Constructor of MainViewModel.</summary>
        public DetailViewModel()
        {
            Init.CreateDataDirectoryIfNotExists();
            //Creation of a list of 5 JobBackup
            _listOfJobBackup = Init.CreateJobBackupList();
            _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
            _view = new View();
            _configuration = Init.LoadConfiguration();
        }
        public void Start()
        {
            JsonReadWriteModel JSonReaderWriter = new JsonReadWriteModel();
            //Check the number of jobBackup in the list and update the count

        }
        public List<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set => _listOfJobBackup = value; }
    }
}
