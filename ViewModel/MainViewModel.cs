using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasySave
{
    public class MainViewModel
    {
        //Pick up the console input value 
        private List<JobBackup> listOfJobBackup = Init.CreateJobBackupList();
        View view = new View();

        public void Start()
        {
            Boolean app = true;
            while (app is true)
            {
                view.DisplayIntroduction();
                //Menu Display
                int input = CheckInput();
                //Cases of menu
                switch (input)
                {
                    //Start
                    case 1:
                        view.DisplayChooseAction();
                        input = CheckInput();
                        switch (input)
                        {
                            //Create a new job backup
                            case 1:
                                //Check if the number of saves is inferior or equal to 5
                                view.DisplayChooseSave();
                                input = CheckInput();
                                //Choose if you want a total or sequential save
                                switch (input)
                                {
                                    //For Total Save
                                    case 1:
                                        Console.Clear();
                                        //Insert in the list the job
                                        view.Display("");
                                        view.Display("  Choose a space from 1 to 5");
                                        DisplayJobBackup();
                                        input = CheckInput() - 1;
                                        while (input > 5)
                                        {
                                            view.Display("  Not a choice, restarting");
                                            input = CheckInput() -1;

                                        }
                                        view.Display("  Write the name of the new save");
                                        listOfJobBackup[input].Label = Console.ReadLine();
                                        view.Display("  Write the name of the source directory");
                                        listOfJobBackup[input].SourceDirectory = CheckSourceDirectory();
                                        view.Display("  Write the name of the destination directory");
                                        listOfJobBackup[input].DestinationDirectory = Console.ReadLine();
                                        listOfJobBackup[input].IsDifferential = false;
                                        Console.Clear();
                                        view.Display("  Created!");
                                        view.Display("");
                                        break;

                                    //For Differential Save
                                    case 2:
                                        Console.Clear();
                                        //Insert in the list the job
                                        view.Display("  Veuillez choisir un emplacement");
                                        DisplayJobBackup();
                                        input = CheckInput() -1;
                                        while (input > 5)
                                        {
                                            view.Display("  Not a choice, restarting");
                                            input = CheckInput() -1;
                                        }
                                        view.Display("  Write the name of the new save");
                                        listOfJobBackup[input].Label = Console.ReadLine();
                                        view.Display("  Write the name of the source directory");
                                        listOfJobBackup[input].SourceDirectory = CheckSourceDirectory();
                                        view.Display("  Write the name of the destination directory");
                                        listOfJobBackup[input].DestinationDirectory = Console.ReadLine();
                                        listOfJobBackup[input].IsDifferential = true;
                                        Console.Clear();
                                        view.Display("  "+ listOfJobBackup[input].Label + " Created!");
                                        view.Display("");
                                        break;
                                    //Return to the start
                                    case 3:
                                        break;
                                }
                                break;
                            //To delete a job backup
                            case 2:
                                DeleteSave();
                                Console.Clear();
                                view.Display("  Deleted!");
                                break;
                            //Execute one job backup
                            case 3:
                                view.Display("  Veuillez choisir un job");
                                DisplayJobBackup();
                                input = CheckInput() - 1;
                                Console.Clear();
                                view.Display("  Executing " + listOfJobBackup[input] + "\n");
                                listOfJobBackup[input].Execute();
                                view.Display(" Successfully done! \n");
                                break;

                            //Execute all jobs backup
                            case 4:
                                foreach (JobBackup item in listOfJobBackup)
                                {
                                    item.Execute();
                                }
                                Console.Clear();
                                view.Display("  Done!");
                                break;
                            case 5:
                                break;
                            //Warn
                            default:
                                view.Display("  Put a number between 1 and 5");
                                input = CheckInput();
                                break;
                        }
                        break;
                    //Option
                    case 2:
                        view.DisplayChooseLanguage();
                        input = CheckInput();
                        switch (input)
                        {
                            //English
                            case 1:
                                Console.Clear();
                                view.Display("  Done!");
                                break;
                            //French
                            case 2:
                                Console.Clear();
                                view.Display("  Done!");
                                break;
                            //Warning
                            default:
                                view.Display("  Put a number between 1 and 2");
                                input = CheckInput();
                                break;
                        }
                        break;
                    case 3:
                        app = false;
                        break;
                    //Warning
                    default:
                        view.Display("  Put a number between 1 and 3");
                        input = CheckInput();
                        break;
                }
            }
        }
            //to delete a job backup
            private void DeleteSave()
            {
                view.Display("  Which save do you whish to delete ?");
                DisplayJobBackup();
                view.Display("  Other  - Return to start");
                int input = CheckInput() - 1;
                switch (input)
                {
                    default:
                        Start();
                        break;
                }
                //Choose the job backup in the list
                listOfJobBackup[input].Reset();
            }
            private void DisplayJobBackup()
            {
                int max = 1;
                foreach (JobBackup i in listOfJobBackup)
                {
                    String type = "";
                    if (i.IsDifferential is true && i.Label != "")
                    {
                        type = "Differential";
                        view.Display("  " + max + "  - Label: " + i.Label + " SourceDirectory: " + i.SourceDirectory + " DestinationDirectory:" + i.DestinationDirectory + " Type:" + type);
                        max++;
                    }
                    else if (i.IsDifferential is false && i.Label != "")
                    {
                        type = "Total";
                        view.Display("  " + max + "  - Label: " + i.Label + " SourceDirectory: " + i.SourceDirectory + " DestinationDirectory:" + i.DestinationDirectory + " Type:" + type);
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
                    int saisie = int.Parse(letter) ;
                    test = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("  Please put a number");
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
                    Console.WriteLine("  Directory not found, try again");
                }
            }
            return letter;
        }
    }
}

