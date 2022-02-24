using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteInterface
{
    /// <summary>
    /// Logique d'interaction pour SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        private SettingsViewModel _settingsViewModel;
        private static SettingsView _instance;

        //settings view model
        public SettingsView()
        {
            InitializeComponent();
            _settingsViewModel = new SettingsViewModel();
            this.DataContext = _settingsViewModel;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }


        private void btnValid_Click(object sender, RoutedEventArgs e)
        {
           
                String language = String.Empty;
                if (comboBoxLanguages.SelectedIndex == 0) language = "fr-FR";
                if (comboBoxLanguages.SelectedIndex == 1) language = "en-US";
                _settingsViewModel.SaveSettings(language);
                this.NavigationService.GoBack();
            
        }


        //prevent spam click when using of goback()
        public static SettingsView GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingsView();
            }
            return _instance;
        }
    }
}
