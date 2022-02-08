﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace EasySave
{
    public class Configuration : INotifyPropertyChanged
    {
        //Attributes for the configuration of the software
        private String _language;
        private String _businessSoftware;
        private String[] _extensions;

        private static Configuration _instance;
        public const String DEFAULT_CONFIG_FILE_PATH = @"C:\EasySave\Configuration.json";

        public event PropertyChangedEventHandler PropertyChanged;

        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged("Language");
            }
        }
        public string BusinessSoftware
        {
            get => _businessSoftware;
            set
            {
               _businessSoftware = value;
                OnPropertyChanged("BusinessSoftware");
            } 
        }
        public string[] Extensions
        {
            get => _extensions;
            set
            { 
                _extensions = value;
                OnPropertyChanged("Extensions");
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

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
