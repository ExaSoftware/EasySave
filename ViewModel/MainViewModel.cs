using System;
using System.Collections.Generic;
using System.Text;
using static EasySave.View;

namespace EasySave
{
    public class MainViewModel
    {
        //Pick up the console input value 
        private List <JobBackup> listOfJobBackup = new List<JobBackup>(5);

        public void Start() {
            //Menu Display
            DisplayIntroduction();
            int input = Convert.ToInt32(Console.ReadLine());
            //Cases of menu
            switch (input)
            {
                //Start
                case 1:
                    DisplayChooseAction();
                    input = Convert.ToInt32(Console.ReadLine());
                    switch (input)
                    {
                        //Create a new job backup
                        case 1:
                            //Check if the number of saves is inferior or equal to 5

                            DisplayChooseSave();
                            input = Convert.ToInt32(Console.ReadLine());
                            //Choose if you want a total or sequential save
                            switch (input)
                            {
                                //For Total Save
                                case 1:
                                    //Insert in the list the job
                                    Display("  Choose a space from 1 to 5");
                                    DisplayJobBackup();
                                    input = Convert.ToInt32(Console.ReadLine())-1;
                                    if (input > 5)
                                    {
                                        Display("  Not a choice, restarting");
                                        Start();

                                    }
                                    Display("  Write the name of the source directory");
                                    listOfJobBackup[input].Label = Console.ReadLine();
                                    Display("  Write the name of the source directory");
                                    listOfJobBackup[input].SourceDirectory = Console.ReadLine();
                                    Display("  Write the name of the destination directory");
                                    listOfJobBackup[input].DestinationDirectory = Console.ReadLine();
                                    listOfJobBackup[input].IsDifferential = false;
                                    Display("  Created!");
                                    Display("");
                                    Start();
                                    break;

                                //For Differential Save
                                case 2:
                                    //Insert in the list the job
                                    Display("  Veuillez choisir un emplacement");
                                    input = Convert.ToInt32(Console.ReadLine());
                                    if (input > 5)
                                    {
                                        Display("  Not a choice, restarting");
                                        Start();
                                    }
                                    Display("Write the name of the new save");
                                    listOfJobBackup[input].Label = Console.ReadLine();
                                    Display("  Write the name of the source directory");
                                    listOfJobBackup[input].SourceDirectory = Console.ReadLine();
                                    Display("  Write the name of the destination directory");
                                    listOfJobBackup[input].DestinationDirectory = Console.ReadLine();
                                    listOfJobBackup[input].IsDifferential = true;
                                    Console.Clear();
                                    Display("  Created!");
                                    Display("");
                                    Start();
                                    break;
                            }
                            break;
                        //To delete a job backup
                        case 2:
                            DeleteSave();
                            Display("  Deleted!");
                            Start();
                            break;
                        //Execute one job backup
                        case 3:
                            Display("  Veuillez choisir un job");
                            DisplayJobBackup();
                            input = Convert.ToInt32(Console.ReadLine());
                            listOfJobBackup[input].Execute();
                            Display("  Done!");
                            Start();            
                            break;

                        //Execute all jobs backup
                        case 4:
                            foreach (JobBackup item in listOfJobBackup)
                            {
                                item.Execute();
                            }
                            Display("  Done!");
                            Start();
                            break;
                        //Warn
                        default:
                            Display("  Put a number between 1 and 4");
                            break;
                    }
                    break;
                   //Option
                case 2:
                    DisplayChooseLanguage();
                    input = Convert.ToInt32(Console.ReadLine());
                    switch (input)
                    {
                        //English
                        case 1:
                            Display("  Done!");
                            break;
                        //French
                        case 2:
                            Display("  Done!");
                            break;
                        //Warning
                        default:
                            Display("  Put a number between 1 and 2");
                            break;
                    }
                    break;
                case 3:
                    break;
                //Warning
                default:
                    Display("  Put a number between 1 and 3");
                    break;
            }
        }
        //to delete a job backup
        private void DeleteSave()
        {
            Display("  Which save do you whish to delete ?");
            DisplayJobBackup();
            int input = Convert.ToInt32(Console.ReadLine())-1;

            //Choose the job backup in the list
            listOfJobBackup[input].Reset();

        }
        private void DisplayJobBackup()
        {
            int max = 1;
            foreach (JobBackup i in listOfJobBackup)
            {
                String type = "";
                if(i.IsDifferential is true && i.Label != null)
                {
                    type = "Differential";
                    Display("  " + max + "  - Label: " + i.Label + " SourceDirectory: " + i.SourceDirectory + " DestinationDirectory:" + i.DestinationDirectory + " Type:" + type);
                    max++;
                }
                else if (i.IsDifferential is false && i.Label !=null)
                {
                    type = "Total";
                    Display("  " + max + "  - Label: " + i.Label + " SourceDirectory: " + i.SourceDirectory + " DestinationDirectory:" + i.DestinationDirectory + " Type:" + type);
                    max++;
                }
            }
        }
    }
}

