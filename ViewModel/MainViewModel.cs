using System;
using System.Collections.Generic;
using System.Text;
using static EasySave.View;

namespace EasySave
{
    public class MainViewModel
    {
        //Pick up the console input value 
        private String _sourceDirectory = "";
        private String _destinationDirectory = "";
        private String _nameSave = "";


        public void Start() {
            //Menu Display
            DisplayIntroduction();
            int input = Convert.ToInt32(Console.ReadLine());

            //Cases of menu
            switch (input)
            {
                //Start of the copy
                case 1:
                    DisplayChooseSave();
                    //ChooseSave
                    input = Convert.ToInt32(Console.ReadLine());
                    switch (input)
                    {
                        //For Total Save
                        case 1:
                            Parameters();
                            Display("Done!");
                            break;
                        //For Sequential Save
                        case 2:
                            Parameters();
                            Display("Done!");
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
                            Display("Done!");
                            break;
                        //French
                        case 2:
                            Display("Done!");
                            break;
                        //Warning
                        default:
                            Display("Put a number between 1 and 2");
                            break;
                    }
                    break;
                case 3:
                    break;
                //Warning
                default:
                    Display("Put a number between 1 and 3");
                    break;
            }
        }
        //to configure the save
        private void Parameters()
        {
            Display("Write the name of the source directory");
            _sourceDirectory = Console.ReadLine();
            Display("Write the name of the destination directory");
            _destinationDirectory = Console.ReadLine();
            Display("Write the name of the new save");
            _nameSave = Console.ReadLine();
        }
    }
}

