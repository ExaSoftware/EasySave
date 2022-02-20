using System;
using System.IO;
using System.Text.Json;

namespace EasySave
{
    public class Configuration
    {
        //Attributes for the configuration of the software
        private String _language;
        private String _businessSoftware;
        private String[] _extensions;
        private string _logFormat;
        private ulong _sizeLimit;

        private static Configuration _instance;
        public const String DEFAULT_CONFIG_FILE_PATH = @"C:\EasySave\Configuration.json";
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
            }
        }
        public string BusinessSoftware
        {
            get => _businessSoftware;
            set
            {
                _businessSoftware = value;
            }
        }
        public string[] Extensions
        {
            get => _extensions;
            set
            {
                _extensions = value;
            }
        }

        public string LogFormat { get => _logFormat; set => _logFormat = value; }
        public ulong SizeLimit { get => _sizeLimit; set => _sizeLimit = value; }

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
