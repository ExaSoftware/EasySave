using EasySave.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace EasySave
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
            //Clear errors
            ClearErrorFields();
            //Verify fields and fill errors dictionnary if necessary
            CheckFields();
            //Showing errors if necessary
            if (_settingsViewModel.Errors.Count > 0)
            {
                ShowErrorsFromSettingsViewModel();
            }
            else
            {
                String language = String.Empty;
                String businessSoftware = txtBoxBusinessSoftwarePath.Text;
                String[] extensions = txtBoxExtensionsList.Text.Split(";");
                string logFormat = String.Empty;
                if (comboBoxLogFormat.SelectedIndex == 0) logFormat = "json";
                if (comboBoxLogFormat.SelectedIndex == 1) logFormat = "xml";
                if (comboBoxLanguages.SelectedIndex == 0) language = "fr-FR";
                if (comboBoxLanguages.SelectedIndex == 1) language = "en-US";
                _settingsViewModel.SaveSettings(language, businessSoftware, extensions, logFormat);
                this.NavigationService.Navigate(new MainView());
                _settingsViewModel.SaveSettings(language, businessSoftware, extensions);
                this.NavigationService.GoBack();
            }
        }

        private void CheckFields()
        {
            _settingsViewModel.CheckExtension(txtBoxExtensionsList.Text);
        }

        /// <summary>
        /// Method which read the errors dictionnary of the Setting Model and show it if necessary. Useful for showing error more dynamically to the user.
        /// </summary>
        private void ShowErrorsFromSettingsViewModel()
        {
            foreach (KeyValuePair<string, string> entry in _settingsViewModel.Errors)
            {
                if (entry.Key == "extensionError")
                {
                    extensionError.Text = _settingsViewModel.Errors["extensionError"]; extensionError.Visibility = Visibility.Visible;
                }
            }
            _settingsViewModel.Errors.Clear();
        }

        /// <summary>
        /// Clear UI errors
        /// </summary>
        private void ClearErrorFields()
        {
            extensionError.Text = ""; extensionError.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Method which called when the source directory text changed. Useful for showing error more dynamically to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxExtensionsListChanged(object sender, TextChangedEventArgs e)
        {
            //Ensure textbox is property loaded
            if (this.IsLoaded)
            {
                //Check and show extensions error
                _settingsViewModel.CheckExtension(txtBoxExtensionsList.Text);
                if (_settingsViewModel.Errors.ContainsKey("extensionError"))
                {
                    extensionError.Text = _settingsViewModel.Errors["extensionError"]; extensionError.Visibility = Visibility.Visible;
                }
                else
                {
                    extensionError.Text = ""; extensionError.Visibility = Visibility.Collapsed;
                }
                _settingsViewModel.Errors.Remove("extensionError");
            }
        }

        private void txtBoxExtensionsList_LostFocus(object sender, RoutedEventArgs e)
        {
            //Check and show the error if necessary
            _settingsViewModel.CheckExtension(txtBoxExtensionsList.Text);
            if (_settingsViewModel.Errors.ContainsKey("extensionError"))
            {
                extensionError.Text = _settingsViewModel.Errors["extensionError"]; extensionError.Visibility = Visibility.Visible;
            }
            else
            {
                extensionError.Text = ""; extensionError.Visibility = Visibility.Collapsed;
            }
            _settingsViewModel.Errors.Remove("extensionError");
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
