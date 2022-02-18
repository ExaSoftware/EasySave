using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace EasySave.ViewModel
{
    class SettingsViewModel
    {
        private int _selectedLanguage;
        private int _selectedLogFormat;
        private string _extensions;
        private string _businessSoftware;
        private ResourceManager _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

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

            if (String.Equals(App.Configuration.LogFormat, "json"))
            {
                _selectedLogFormat = 0;
            }
            if (String.Equals(App.Configuration.LogFormat, "xml"))
            {
                _selectedLogFormat = 1;
            }

            //Set the attributes to the convert the list into a string fot the display in the field in view
            if (App.Configuration.Extensions != null)
            {
                _extensions = String.Join(";", App.Configuration.Extensions);
            }
        }

        public void SaveSettings(String language, String businessSoftware, String[] extensions, string logFormat)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language, false);
            App.Configuration.Language = language;
            App.Configuration.BusinessSoftware = businessSoftware;
            App.Configuration.Extensions = extensions;
            App.Configuration.LogFormat = logFormat;
            App.Configuration.Save();
        }
        public void CheckExtension(string extensions)
        {
            //Check if the list doesnt have multiple ;;
            string pattern = @"\.[-!$%^&*()_+|~=`\\#{}\[\]:\<>?,\/+\w*\d*\s*àèìòùÀÈÌÒÙáéíóúýÁÉÍÓÚÝâêîôûÂÊÎÔÛãñõÃÑÕäëïöüÿÄËÏÖÜŸçÇßØøÅåÆæœ]*\.+[\w*\d*\s*àèìòùÀÈÌÒÙáéíóúýÁÉÍÓÚÝâêîôûÂÊÎÔÛãñõÃÑÕäëïöüÿÄËÏÖÜŸçÇßØøÅåÆæœ]*";
            Match result = Regex.Match(extensions, pattern);
            if (result.Success)
            {
                _errors.Add("extensionError", _rm.GetString("extensionFormatError"));
            }
        }
        public int SelectedLanguage { get => _selectedLanguage; set => _selectedLanguage = value; }
        public String Extensions { get => _extensions; set => _extensions = value; }
        public string BusinessSoftware { get => _businessSoftware; set => _businessSoftware = value; }
        public Dictionary<string, string> Errors { get => _errors; set => _errors = value; }
        public int SelectedLogFormat { get => _selectedLogFormat; set => _selectedLogFormat = value; }
    }
}
