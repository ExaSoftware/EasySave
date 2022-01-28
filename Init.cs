using EasySave.Object;
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
            if (!Directory.Exists(@"C:\EasySave"))
            {
                Directory.CreateDirectory(@"C:\EasySave");
            }

            if (!Directory.Exists(@"C:\EasySave\Job-Backup"))
            {
                Directory.CreateDirectory(@"C:\EasySave\Job-Backup");
            }
        }

        public static List<JobBackup> CreateJobBackupList()
        {
            JsonReadWriteModel reader = new JsonReadWriteModel();
            List<JobBackup> parts = new List<JobBackup>();

            if (reader.ReadJobBackup() == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    parts.Add(new JobBackup(i));
                }
                reader.SaveJobBackup(parts);
                return parts;
            }
            else
            {
                parts = reader.ReadJobBackup();
                reader.SaveJobBackup(parts);
                return parts;
            }

        }
    }
}
