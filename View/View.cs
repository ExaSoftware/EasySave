using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Text;

namespace EasySave
{
    public class View
    {
        private ResourceManager _rm;
        public View()
        {
            _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
        }
        //Reusable Function
        public void Display(string text)
        {
            Console.WriteLine(text);
        }

        //Introduction
        public void DisplayIntroduction(){
            Console.WriteLine(String.Format("  {0}", _rm.GetString("menuIntroductionInsert")));
            Console.WriteLine("");
            Console.WriteLine(String.Format("  1  -  {0}", _rm.GetString("menuIntroductionStart")));
            Console.WriteLine(String.Format("  2  -  {0}", _rm.GetString("menuIntroductionOptions")));
            Console.WriteLine(String.Format("  3  -  {0}", _rm.GetString("menuExit")));
            Console.WriteLine("");
        }

        //Display of which save to choose
        public void DisplayChooseAction()
        {
            Console.WriteLine(String.Format("  {0}", _rm.GetString("menuAction")));
            Console.WriteLine("");
            Console.WriteLine(String.Format("  1  -  {0}", _rm.GetString("menuActionCreateJob")));
            Console.WriteLine(String.Format("  2  -  {0}", _rm.GetString("menuActionDeleteJob")));
            Console.WriteLine(String.Format("  3  -  {0}", _rm.GetString("menuActionExecuteOneJob")));
            Console.WriteLine(String.Format("  4  -  {0}", _rm.GetString("menuActionExecuteAllJobs")));
            Console.WriteLine(String.Format("  5  -  {0}", _rm.GetString("menuReturnStart")));
            Console.WriteLine("");
        }

        //Display of which save to choose
        public void DisplayChooseSave()
        {
            Console.WriteLine(String.Format("  {0}", _rm.GetString("menuSaveType")));
            Console.WriteLine("");
            Console.WriteLine(String.Format("  1  -  {0}", _rm.GetString("menuSaveTotal")));
            Console.WriteLine(String.Format("  2  -  {0}", _rm.GetString("menuSaveDifferential")));
            Console.WriteLine(String.Format("  3  -  {0}", _rm.GetString("menuReturnStart")));

            Console.WriteLine("");
        }

        //Display of which save to choose
        public void DisplayChooseLanguage()
        {
            Console.WriteLine(String.Format("  {0}", _rm.GetString("menuLanguageChoose")));
            Console.WriteLine("");
            Console.WriteLine(String.Format("  1  -  {0}", _rm.GetString("menuLanguageEn")));
            Console.WriteLine(String.Format("  2  -  {0}", _rm.GetString("menuLanguageFr")));
            Console.WriteLine("");
        }
    }
}
