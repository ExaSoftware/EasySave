using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave
{
    public class View
    {
        //Reusable Function
        public void Display(string text)
        {
            Console.WriteLine(text);
        }

        //Introduction
        public void DisplayIntroduction(){
            Console.WriteLine("  Insert one of the numbers");
            Console.WriteLine("");
            Console.WriteLine("  1  -  Start");
            Console.WriteLine("  2  -  Options");
            Console.WriteLine("  3  -  Exit");
            Console.WriteLine("");
        }

        //Display of which save to choose
        public void DisplayChooseAction()
        {
            Console.WriteLine("  Choose the action");
            Console.WriteLine("");
            Console.WriteLine("  1  -  Create a new job backup ");
            Console.WriteLine("  2  -  Delete a job backup");
            Console.WriteLine("  3  -  Execute one job backup");
            Console.WriteLine("  4  -  Execute all job backup");
            Console.WriteLine("  5  -  Return to start");
            Console.WriteLine("");
        }

        //Display of which save to choose
        public void DisplayChooseSave()
        {
            Console.WriteLine("  Which save do you whish to use ?");
            Console.WriteLine("");
            Console.WriteLine("  1  -  Total save");
            Console.WriteLine("  2  -  Differential save");
            Console.WriteLine("  3  -  Return to start");

            Console.WriteLine("");
        }

        //Display of which save to choose
        public void DisplayChooseLanguage()
        {
            Console.WriteLine("  Choose the language");
            Console.WriteLine("");
            Console.WriteLine("  1  -  English");
            Console.WriteLine("  2  -  French");
            Console.WriteLine("");
        }
    }
}
