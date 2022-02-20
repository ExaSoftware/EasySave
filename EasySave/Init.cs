using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace EasySave
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
                configuration.LogFormat = "json";
                configuration.Save();
                return configuration;
            }
            else
            {
                String jsonString = File.ReadAllText(Configuration.DEFAULT_CONFIG_FILE_PATH);
                Configuration configuration = JsonSerializer.Deserialize<Configuration>(jsonString);
                //Set the language according to the language in configuration file
                CultureInfo.CurrentUICulture = new CultureInfo(configuration.Language, false);
                return configuration;
            }
        }

        /// <summary>
        /// Create Data Directory of EasySave which contains logs, configuration and saves
        /// </summary>
        public static void CreateDataDirectoryIfNotExists()
        {
            Directory.CreateDirectory(@"C:\EasySave");
            Directory.CreateDirectory(@"C:\EasySave\Job-Backup");
        }

        public static List<JobBackup> CreateJobBackupList()
        {
            List<JobBackup> parts = new List<JobBackup>();

            if (JsonReadWriteModel.ReadJobBackup() == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    parts.Add(new JobBackup(i));
                }
                JsonReadWriteModel.SaveJobBackup(parts);
                return parts;
            }
            else
            {
                parts = JsonReadWriteModel.ReadJobBackup();
                JsonReadWriteModel.SaveJobBackup(parts);
                return parts;
            }

        }
    }
}
