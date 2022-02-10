using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EasySave.ViewModel
{
    class SettingsViewModel
    {
        private int _selectedLanguage;
        private String _extensions;
        private String _businessSoftware;

        public SettingsViewModel()
        {

            _businessSoftware = App.Configuration.BusinessSoftware;
            //Set the attributes to set the selected language in the combobox
            if (String.Equals(App.Configuration.Language, "fr-FR"))
            {
                _selectedLanguage = 0;
            }
            if (String.Equals(App.Configuration.Language, "en-US"))
            {
                _selectedLanguage = 1;
            }

            //Set the attributes to the convert the list into a string fot the display in the field in view
            if (App.Configuration.Extensions != null)
            {
                _extensions = String.Join(";", App.Configuration.Extensions);
            }
        }

        public void SaveSettings(String language, String businessSoftware, String[] extensions)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language, false);
            App.Configuration.Language = language;
            App.Configuration.BusinessSoftware = businessSoftware;
            App.Configuration.Extensions = extensions;
            App.Configuration.Save();
        }
        public int SelectedLanguage { get => _selectedLanguage; set => _selectedLanguage = value; }
        public String Extensions { get => _extensions; set => _extensions = value; }
        public string BusinessSoftware { get => _businessSoftware; set => _businessSoftware = value; }
    }
}
