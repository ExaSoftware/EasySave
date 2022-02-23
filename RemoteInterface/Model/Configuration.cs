using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace RemoteInterface
{
    public class Configuration
    {
        //Attributes for the configuration of the software
        private String _language;

        private static Configuration _instance;
        public const String DEFAULT_CONFIG_FILE_PATH = @"C:\EasySave\Remote\Configuration.json";
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
            }
        }

        /// <summary>
        /// Serialize and save Configuration object in json file
        /// </summary>
        public void Save()
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            String json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(DEFAULT_CONFIG_FILE_PATH, json);
        }

        public void CreateConfigurationFile()
        {
            Directory.CreateDirectory(@"C:\EasySave\Remote");
            File.Create(DEFAULT_CONFIG_FILE_PATH).Close();
        }

        /// <summary>
        /// Singleton which get Configuration object
        /// </summary>
        /// <returns></returns>
        public static Configuration GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Configuration();
            }
            return _instance;
        }
    }
}
