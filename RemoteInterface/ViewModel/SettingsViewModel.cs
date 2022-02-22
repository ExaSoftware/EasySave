using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace RemoteInterface
{
    class SettingsViewModel
    {
        private int _selectedLanguage;
        private ResourceManager _rm = new ResourceManager("RemoteEasySave.Resources.Strings", Assembly.GetExecutingAssembly());
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public SettingsViewModel()
        {
            //Set the attributes to set the selected language in the combobox
            if (String.Equals(App.Configuration.Language, "fr-FR"))
            {
                _selectedLanguage = 0;
            }
            if (String.Equals(App.Configuration.Language, "en-US"))
            {
                _selectedLanguage = 1;
            }
        }

        public void SaveSettings(String language)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language, false);
            App.Configuration.Language = language;
            App.Configuration.Save();
        }

        public int SelectedLanguage { get => _selectedLanguage; set => _selectedLanguage = value; }
        public Dictionary<string, string> Errors { get => _errors; set => _errors = value; }

    }
}
