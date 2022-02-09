using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EasySave.ViewModel
{
    class SettingsViewModel
    {
        private Configuration _configuration;
        private int _selectedLanguage;
        private String _extensions;

        public SettingsViewModel()
        {
            _configuration = Init.LoadConfiguration();

            //Set the attributes to set the selected language in the combobox
            if (String.Equals(_configuration.Language, "fr-FR"))
            {
                _selectedLanguage = 0;
            }
            if (String.Equals(_configuration.Language, "en-US"))
            {
                _selectedLanguage = 1;
            }

            //Set the attributes to the convert the list into a string fot the display in the field in view
            if (_configuration.Extensions != null)
            {
                _extensions = String.Join(";", _configuration.Extensions);
            }
        }

        public void SaveSettings(String language, String businessSoftware, String[] extensions)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language, false);
            _configuration.Language = language;
            _configuration.BusinessSoftware = businessSoftware;
            _configuration.Extensions = extensions;
            _configuration.Save();
        }

        public Configuration Configuration { get => _configuration; set => _configuration = value; }
        public int SelectedLanguage { get => _selectedLanguage; set => _selectedLanguage = value; }
        public String Extensions { get => _extensions; set => _extensions = value; }
    }
}
