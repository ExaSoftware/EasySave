using EasySave.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace EasySave.ViewModel
{
    public static class Init
    {
        /// <summary>
        /// Load the configuration file of the application
        /// </summary>
        /// <returns>A Configuration Object</returns>
        public static Configuration LoadConfiguration()
        {
            if (!File.Exists(Configuration.DEFAULT_CONFIG_FILE_PATH))
            {
                Configuration configuration = Configuration.GetInstance();
                configuration.CreateConfigurationFile();
                configuration.Language = Thread.CurrentThread.CurrentUICulture.Name;
                configuration.LogsPath = @"C:\EasySave\Logs";
                configuration.Save();
                return configuration;
            }
            else
            {
                String jsonString = File.ReadAllText(Configuration.DEFAULT_CONFIG_FILE_PATH);
                return JsonSerializer.Deserialize<Configuration>(jsonString);
            }
        }

        public static void CreateDataDirectoryIfNotExists()
        {
            if (!Directory.Exists(@"C:\EasySave"))
            {
                Directory.CreateDirectory(@"C:\EasySave");
            }
        }
    }
}
