﻿using EasySave.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;


namespace EasySave
{
    /// <summary>
    ///  MainViewModel is responsible to link the display of the view to the work of the server
    /// </summary>
    public class MainViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;
        private ResourceManager _rm;
        private View _view;
        private Configuration _configuration;

        ///  <summary>Constructor of MainViewModel.</summary>
        public MainViewModel()
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
            Boolean app = true;
            while (app is true)
            {
                Boolean isError = false;
                _view.DisplayIntroduction();
                //Menu Display
                int input = CheckInput();
                //Cases of menu
                switch (input)
                {
                    //Start
                    case 1:
                        _view.DisplayChooseAction();
                        input = CheckInput();
                        switch (input)
                        {
                            //Create a new job backup
                            case 1:
                                //Check if the number of saves is inferior or equal to 5
                                _view.DisplayChooseSave();
                                input = CheckInput();
                                //Choose if you want a total or sequential save
                                switch (input)
                                {
                                    //For Total Save
                                    case 1:
                                        JobCreation(input, JSonReaderWriter);
                                        break;

                                    //For Differential Save
                                    case 2:
                                        JobCreation(input, JSonReaderWriter);
                                        break;
                                    //Return to the start
                                    case 3:
                                        Console.Clear();
                                        break;
                                }
                                break;
                            //To delete a job backup
                            case 2:
                                DeleteSave();
                                //Save the JobBackup list in JSON file
                                JSonReaderWriter.SaveJobBackup(_listOfJobBackup);
                                Console.Clear();
                                _view.Display(String.Format("  {0}", _rm.GetString("backupJobDeleted")) + Environment.NewLine);
                                break;
                            //Execute one job backup
                            case 3:
                                Console.Clear();
                                _view.Display("");
                                _view.Display(String.Format("  {0}", _rm.GetString("menuChooseJob")) + Environment.NewLine);
                                DisplayJobBackup();
                                input = CheckInput() - 1;
                                while (input > 4)
                                {
                                    _view.Display(String.Format("  {0}", _rm.GetString("menuChoiceError")));
                                    input = CheckInput() - 1;
                                }
                                Console.Clear();
                                if (String.IsNullOrEmpty(_listOfJobBackup[input].Label)) //If the job backup isn't created by the user
                                {
                                    _view.Display(_rm.GetString("errorJobBackupNotCreated") + Environment.NewLine);
                                }
                                else
                                {
                                    _view.Display(String.Format(_rm.GetString("runningBackupJob"), _listOfJobBackup[input].Label));
                                    //Check if there's an error with the parameters of the save and launch the save

                                    if (_listOfJobBackup[input].Execute(new List<string>()))
                                    {
                                        _view.Display(String.Format(_rm.GetString("saveError"), _listOfJobBackup[input].Label) + Environment.NewLine);
                                        isError = true;
                                        break;
                                    }
                                    if (!isError)
                                    {
                                        _view.Display(String.Format(_rm.GetString("backupJobExecutionSuccessfullyDone"), _listOfJobBackup[input].Label) + Environment.NewLine);
                                    }

                                }
                                break;


                            //Execute all jobs backup
                            case 4:
                                Console.Clear();
                                //Launch every save
                                foreach (JobBackup item in _listOfJobBackup)
                                {
                                    if (!String.IsNullOrEmpty(item.Label)) _view.Display(String.Format(_rm.GetString("runningBackupJob"), item.Label));
                                    //Check if there's an error with the parameters of the save
                                    if (!String.IsNullOrEmpty(item.Label) && item.Execute(new List<string>()))
                                    {
                                        _view.Display(String.Format(_rm.GetString("saveError"), item.Label) + Environment.NewLine);
                                        isError = true;
                                        break;
                                    }
                                }
                                if (!isError)
                                {
                                    _view.Display(_rm.GetString("sequentialExecutionSuccessfullyDone") + Environment.NewLine);
                                }
                                break;
                            case 5:
                                Console.Clear();
                                break;
                            //Warn
                            default:
                                _view.Display(String.Format("  {0}", _rm.GetString("menuWarn15")));
                                input = CheckInput();
                                break;
                        }
                        break;
                    //Option
                    case 2:
                        _view.DisplayChooseLanguage();
                        input = CheckInput();
                        switch (input)
                        {
                            //Change language to English
                            case 1:
                                Console.Clear();
                                LanguageManager.ChangeLanguage("en-US", _configuration);
                                _view.Display(String.Format(_rm.GetString("languageChangedSuccess"), "en-US") + Environment.NewLine);
                                break;
                            //Change language to French
                            case 2:
                                Console.Clear();
                                LanguageManager.ChangeLanguage("fr-FR", _configuration);
                                _view.Display(String.Format(_rm.GetString("languageChangedSuccess"), "fr-FR") + Environment.NewLine);
                                break;
                            //Warning
                            default:
                                _view.Display(String.Format("  {0}", _rm.GetString("menuWarn12")));
                                input = CheckInput();
                                break;
                        }
                        break;
                    case 3:
                        app = false;
                        break;
                    //Warning
                    default:
                        _view.Display(String.Format("  {0}", _rm.GetString("menuWarn14")));
                        input = CheckInput();
                        break;
                }
            }
        }

        ///  <summary>To delete a job backup.</summary>
        private void DeleteSave()
        {
            Console.Clear();
            _view.Display(String.Format("  {0}", _rm.GetString("menuAskJobDelete")) + Environment.NewLine);
            DisplayJobBackup();
            _view.Display(String.Format("  {0}", _rm.GetString("menuOtherReturnStart")));

            //Choose the job backup in the list
            int input = CheckInput() - 1;
            if (input > 4)
            {
                Console.Clear();
                Start();
            }
            else
            {
                _listOfJobBackup[input] = _listOfJobBackup[input].Reset();
            }
        }

        ///  <summary>Display every save with their parameters</summary>
        private void DisplayJobBackup()
        {
            JsonReadWriteModel reader = new JsonReadWriteModel();
            //Save the JobBackup list in JSON file
            _listOfJobBackup = reader.ReadJobBackup();
            int max = 1;
            foreach (JobBackup i in _listOfJobBackup)
            {
                String type = "";
                if (i.IsDifferential is true)
                {
                    type = _rm.GetString("jobTypeDifferential");
                    _view.Display(String.Format("  {0}  -  {1} '{2}' {3} '{4}' {5} '{6}' {7} '{8}'", max, _rm.GetString("jobLabel"), i.Label, _rm.GetString("jobSourceDirectory"), i.SourceDirectory, _rm.GetString("jobDestinationDirectory"), i.DestinationDirectory, _rm.GetString("jobType"), type));
                    max++;
                }
                else if (i.IsDifferential is false)
                {
                    type = _rm.GetString("jobTypeComplete");
                    _view.Display(String.Format("  {0}  -  {1} '{2}' {3} '{4}' {5} '{6}' {7} '{8}'", max, _rm.GetString("jobLabel"), i.Label, _rm.GetString("jobSourceDirectory"), i.SourceDirectory, _rm.GetString("jobDestinationDirectory"), i.DestinationDirectory, _rm.GetString("jobType"), type));
                    max++;
                }
            }
        }

        ///  <summary>Verify if the input is an integer</summary>
        ///  <returns>The checked input as an integer</returns> 
        private int CheckInput()
        {
            Boolean test = false;
            String letter = "";

            while (test != true)
            {
                letter = Console.ReadLine();
                try
                {
                    int saisie = int.Parse(letter);
                    test = true;
                }
                catch (FormatException)
                {
                    _view.Display(String.Format("  {0}", _rm.GetString("errorFormatNumber")));
                }

            }
            return Convert.ToInt32(letter);
        }

        ///  <summary>Verify if the source directory exists</summary>
        ///  <returns>The checked source directory</returns> 
        private String CheckSourceDirectory()
        {
            Boolean test = false;
            String letter = "";
            while (test != true)
            {
                letter = Console.ReadLine();

                if (Directory.Exists(letter))
                {
                    test = true;
                    return letter;
                }
                else
                {
                    _view.Display(String.Format("  {0}", _rm.GetString("errorDirectoryNotFound")));
                }
            }
            return letter;
        }

        ///  <summary>Verify if the user input and convert it</summary>
        ///  <returns>Converted user input to integer</returns> 
        private String GetInputJobLabel()
        {
            string entry;
            do
            {
                entry = Console.ReadLine();
                if (String.IsNullOrEmpty(entry))
                {
                    _view.Display(String.Format("  {0}", _rm.GetString("emptyInputJobLabel")));
                }
            } while (String.IsNullOrEmpty(entry));
            return entry;
        }

        ///  <summary>Create the job</summary>
        private void JobCreation(int input, JsonReadWriteModel JSonReaderWriter)
        {
            Console.Clear();
            //Keep the choise of total or differential save
            bool choosen;
            if (input == 1)
            {
                choosen = false;
            }
            else
            {
                choosen = true;
            }
            _view.Display("");
            //Insert in the list the job
            _view.Display(String.Format("  {0}", _rm.GetString("menuChooseSpace")) + Environment.NewLine);
            DisplayJobBackup();
            input = CheckInput() - 1;
            //Check if the input is in the list
            while (input > 4)
            {
                _view.Display(String.Format("  {0}", _rm.GetString("menuChoiceError")));
                input = CheckInput() - 1;
            }
            //Display and choose parameters
            _view.Display(String.Format("  {0}", _rm.GetString("menuJobLabel")));
            _listOfJobBackup[input].Label = GetInputJobLabel();
            _view.Display(String.Format("  {0}", _rm.GetString("menuJobSource")));
            _listOfJobBackup[input].SourceDirectory = CheckSourceDirectory();
            _view.Display(String.Format("  {0}", _rm.GetString("menuJobDestination")));
            _listOfJobBackup[input].DestinationDirectory = Console.ReadLine();
            _listOfJobBackup[input].IsDifferential = choosen;
            //Save the JobBackup list in JSON file
            JSonReaderWriter.SaveJobBackup(_listOfJobBackup);
            Console.Clear();
            _view.Display(String.Format(_rm.GetString("jobBackupSuccessCreated"), _listOfJobBackup[input].Label) + Environment.NewLine);
            //_view.Display("  " + _listOfJobBackup[input].Label + _rm.GetString("jobBackupSuccessCreated"));
            _view.Display("");
        }
    }
}

