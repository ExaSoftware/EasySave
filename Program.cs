using EasySave.Object;
using EasySave.Util;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace EasySave
{
    public class Program
    {
        static void Main(string[] args)
        {
            Init.CreateDataDirectoryIfNotExists();
            //Configuration configuration = Init.LoadConfiguration();
            //LanguageManager.ChangeLanguage("fr-FR", configuration);
            //ResourceManager resourceManager = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
            //Console.WriteLine(resourceManager.GetString("welcome"));
            //LanguageManager.ChangeLanguage("en-US", configuration);
            //Console.WriteLine(resourceManager.GetString("welcome"));
            MainViewModel menu= new MainViewModel();
            menu.Start();
        }
    }
}