using EasySave.Util;
using EasySave.ViewModel;
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

namespace EasySave
{
    /// <summary>
    /// Logique d'interaction pour SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        private SettingsViewModel settingsViewModel;
        private static SettingsView _instance;

        //settings view model
        public SettingsView()
        {
            InitializeComponent();
            settingsViewModel = new SettingsViewModel();
            this.DataContext = settingsViewModel;

            if (String.Equals(settingsViewModel.Configuration.Language, "fr-FR"))
            {
                comboBoxLanguages.SelectedIndex = 0;
            }
            if (String.Equals(settingsViewModel.Configuration.Language, "en-US"))
            {
                comboBoxLanguages.SelectedIndex = 1;
            }

            String test = String.Join(";", settingsViewModel.Configuration.Extensions);
            Trace.WriteLine(test);
            txtBoxExtensionsList.Text = test;
            

        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new DetailView());
        }

        private void btnSelectSourcePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                txtBoxBusinessSoftwarePath.Text = openFileDialog.FileName;
            }
        }

        private void btnValid_Click(object sender, RoutedEventArgs e)
        {
            String language = String.Empty;
            String businessSoftware = txtBoxBusinessSoftwarePath.Text;
            String[] extensions = txtBoxExtensionsList.Text.Split(";"); 
            if (comboBoxLanguages.SelectedIndex == 0) language = "fr-FR";
            if (comboBoxLanguages.SelectedIndex == 1) language = "en-US";
            settingsViewModel.SaveSettings(language, businessSoftware, extensions);
        }

        //prevent spam click when using of goback()
        /*public static SettingsView GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingsView();
            }
            return _instance;
        }*/
    }
}
