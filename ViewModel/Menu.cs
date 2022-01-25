using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave
{
    class Menu
    {
        public static int Start() {
            //Menu Display
            View.Introduction();

            //Pick up the console input value 
            int input = Convert.ToInt32(Console.ReadLine());

            //Cases of menu
            switch (input)
            {
                //Start
                case 1:
                    return input;
                //Help
                case 2:
                    return input;
                //About the option
                case 3:
                    return input;
                //Option
                case 4:
                    return input;
                //Language
                case 5:
                    return input;
                //Something else choosen
                default:
                    View.Warn();
                    break;
            }
            return 0;
        }
    }
}
