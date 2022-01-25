using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave
{
    public class View
    {
        //Introduction
        public static void Introduction(){
            Console.WriteLine("Insert one of the numbers");
            Console.WriteLine("");
            Console.WriteLine("1    Start");
            Console.WriteLine("2    Help");
            Console.WriteLine("3    Options");
            Console.WriteLine("4    About the application");
            Console.WriteLine("5    Exit");
            Console.WriteLine("");

        }

        //Warning Menu
        public static void Warn() {
            Console.WriteLine("Put a number between 1 and 5");
        }
    }
}
