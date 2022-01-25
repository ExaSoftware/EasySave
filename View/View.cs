using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave
{
    public class View
    {
        //Reusable Function
        public static void Display(string text)
        {
            Console.WriteLine(text);
        }

        //Introduction
        public static void DisplayIntroduction(){
            Console.WriteLine("  Insert one of the numbers");
            Console.WriteLine("");
            Console.WriteLine("  1  -  Start");
            Console.WriteLine("  2  -  Options");
            Console.WriteLine("  3  -  Exit");
            Console.WriteLine("");
        }


        //Display of which save to choose
        public static void DisplayChooseSave()
        {
            Console.WriteLine("  Which save do you whish to use ?");
            Console.WriteLine("");
            Console.WriteLine("  1  -  Total save");
            Console.WriteLine("  2  -  Sequential save");
            Console.WriteLine("");
        }

        //Display of which save to choose
        public static void DisplayChooseLanguage()
        {
            Console.WriteLine("Choose the language");
            Console.WriteLine("");
            Console.WriteLine("  1  -  English");
            Console.WriteLine("  2  -  French");
            Console.WriteLine("");
        }
    }
}
