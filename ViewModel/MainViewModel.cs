﻿using EasySave.Object;
using EasySave.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace EasySave
{
    public class MainViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;
        private ResourceManager _rm;
        private View _view;
        private Configuration _configuration;


        public MainViewModel()
        {
            //Creation of a list of 5 JobBackup
            _listOfJobBackup = Init.CreateJobBackupList();
            _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
            _view = new View();
            _configuration = Init.LoadConfiguration();
        }

        public void Start()
        {
            LogModel JSonReaderWriter = new LogModel();
            Boolean app = true;
            while (app is true)
            {
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
                                        Console.Clear();
                                        //Insert in the list the job
                                        _view.Display("");
                                        _view.Display(_rm.GetString("menuChooseSpace"));
                                        DisplayJobBackup();
                                        input = CheckInput() - 1;
                                        while (input > 5)
                                        {
                                            _view.Display(_rm.GetString("menuChoiceError"));
                                            input = CheckInput() -1;

                                        }
                                        _view.Display(_rm.GetString("menuJobLabel"));
                                        _listOfJobBackup[input].Label = Console.ReadLine();
                                        _view.Display(_rm.GetString("menuJobSource"));
                                        _listOfJobBackup[input].SourceDirectory = CheckSourceDirectory();
                                        _view.Display(_rm.GetString("menuJobDestination"));
                                        _listOfJobBackup[input].DestinationDirectory = Console.ReadLine();
                                        _listOfJobBackup[input].IsDifferential = false;
                                        //Save the JobBackup list in JSON file
                                        JSonReaderWriter.SaveJobBackup(_listOfJobBackup);
                                        Console.Clear();
                                        _view.Display("  " + _listOfJobBackup[input].Label + _rm.GetString("successCreated"));
                                        _view.Display("");
                                        break;

                                    //For Differential Save
                                    case 2:
                                        Console.Clear();
                                        //Insert in the list the job
                                        _view.Display(_rm.GetString("menuChooseSpace"));
                                        DisplayJobBackup();
                                        input = CheckInput() -1;
                                        while (input > 5)
                                        {
                                            _view.Display(_rm.GetString("menuChoiceError"));
                                            input = CheckInput() -1;
                                        }
                                        _view.Display (_rm.GetString("menuJobLabel"));
                                        _listOfJobBackup[input].Label = Console.ReadLine();
                                        _view.Display(_rm.GetString("menuJobSource"));
                                        _listOfJobBackup[input].SourceDirectory = CheckSourceDirectory();
                                        _view.Display(_rm.GetString("menuJobDestination"));
                                        _listOfJobBackup[input].DestinationDirectory = Console.ReadLine();
                                        _listOfJobBackup[input].IsDifferential = true;
                                        //Save the JobBackup list in JSON file
                                        JSonReaderWriter.SaveJobBackup(_listOfJobBackup);
                                        Console.Clear();
                                        _view.Display("  "+ _listOfJobBackup[input].Label + _rm.GetString("successCreated"));
                                        _view.Display("");
                                        break;
                                    //Return to the start
                                    case 3:
                                        break;
                                }
                                break;
                            //To delete a job backup
                            case 2:
                                DeleteSave();
                                //Save the JobBackup list in JSON file
                                JSonReaderWriter.SaveJobBackup(_listOfJobBackup);
                                Console.Clear();
                                _view.Display(_rm.GetString("deleted"));
                                break;
                            //Execute one job backup
                            case 3:
                                _view.Display(_rm.GetString("menuChooseJob"));
                                DisplayJobBackup();
                                input = CheckInput() - 1;
                                Console.Clear();
                                _view.Display(_rm.GetString("executing") + _listOfJobBackup[input].Label + "\n");
                                _listOfJobBackup[input].Execute();
                                _view.Display(String.Format("{0}{1}", _rm.GetString("successfullyDone"), Environment.NewLine));
                                break;

                            //Execute all jobs backup
                            case 4:
                                foreach (JobBackup item in _listOfJobBackup)
                                {
                                    item.Execute();
                                }
                                Console.Clear();
                                _view.Display(_rm.GetString("menuJobLabel"));
                                break;
                            case 5:
                                break;
                            //Warn
                            default:
                                _view.Display(_rm.GetString("menuWarn15"));
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
                            //English
                            case 1:
                                Console.Clear();
                                LanguageManager.ChangeLanguage("en-US", _configuration);
                                _view.Display(_rm.GetString("done"));
                                break;
                            //French
                            case 2:
                                Console.Clear();
                                LanguageManager.ChangeLanguage("fr-FR", _configuration);
                                _view.Display(_rm.GetString("done"));
                                break;
                            //Warning
                            default:
                                _view.Display(_rm.GetString("menuWarn12"));
                                input = CheckInput();
                                break;
                        }
                        break;
                    case 3:
                        app = false;
                        break;
                    //Warning
                    default:
                        _view.Display(_rm.GetString("menuWarn14"));
                        input = CheckInput();
                        break;
                }
            }
        }
            //to delete a job backup
            private void DeleteSave()
            {
                _view.Display(_rm.GetString("menuAskJobDelete"));
                DisplayJobBackup();
                _view.Display(_rm.GetString("menuOtherReturnStart"));
                int input = CheckInput() - 1;
                switch (input)
                {
                    default:
                        Start();
                        break;
                }
                //Choose the job backup in the list
                _listOfJobBackup[input].Reset();
            }
            private void DisplayJobBackup()
            {
                int max = 1;
                foreach (JobBackup i in _listOfJobBackup)
                {
                    String type = "";
                    if (i.IsDifferential is true && i.Label != "")
                    {
                        type = _rm.GetString("jobTypeDifferential");
                        _view.Display(String.Format("  {0}  -  {1} '{2}' {3} '{4}' {5} '{6}' {7} '{8}'", max, _rm.GetString("jobLabel"), i.Label, _rm.GetString("jobSourceDirectory"), i.SourceDirectory, _rm.GetString("jobDestinationDirectory"), i.DestinationDirectory, _rm.GetString("jobType"), type));
                        max++;
                    }
                    else if (i.IsDifferential is false && i.Label != "")
                    {
                        type = _rm.GetString("jobTypeComplete");
                        _view.Display(String.Format("  {0}  -  {1} '{2}' {3} '{4}' {5} '{6}' {7} '{8}'", max, _rm.GetString("jobLabel"), i.Label, _rm.GetString("jobSourceDirectory"), i.SourceDirectory, _rm.GetString("jobDestinationDirectory"), i.DestinationDirectory, _rm.GetString("jobType"), type));
                        max++;
                    }
                }
            }
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
                    test = saisie > _listOfJobBackup.Count ? false : true;
                }
                catch (FormatException)
                {
                    Console.WriteLine(_rm.GetString("errorFormatNumber"));
                }   
                
            }
            return Convert.ToInt32(letter);
        }
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
                    Console.WriteLine(_rm.GetString("errorDirectoryNotFound"));
                }
            }
            return letter;
        }
    }
}
