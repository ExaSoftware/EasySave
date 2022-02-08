using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.ViewModel
{
    class SettingsViewModel
    {
        private Configuration _configuration;

        public SettingsViewModel()
        {
            _configuration = Init.LoadConfiguration();
        }

        public void SaveSettings(String language, String businessSoftware, String[] extensions)
        {
            _configuration.Language = language;
            _configuration.BusinessSoftware = businessSoftware;
            _configuration.Extensions = extensions;
            _configuration.Save();
        }

        public Configuration Configuration { get => _configuration; set => _configuration = value; }
    }
}
