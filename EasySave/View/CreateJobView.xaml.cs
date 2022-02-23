using EasySave.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace EasySave
{
    /// <summary>
    /// Logique d'interaction pour CreateJobView.xaml
    /// </summary>
    public partial class CreateJobView : Page
    {
        private CreateJobViewModel _createJobViewModel;
        public CreateJobView(CreateJobViewModel viewModel)
        {
            this._createJobViewModel = viewModel;
            this.DataContext = viewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Method which open folder browser dialog when user click on the file button to chosse a source directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelectSourceClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            txtBoxSource.Text = folderBrowserDialog.SelectedPath;
        }

        /// <summary>
        /// Method which open folder browser dialog when user click on the file button to chosse a destination directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelectDestinationClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            txtBoxDestination.Text = folderBrowserDialog.SelectedPath;
        }

        /// <summary>
        /// Return to the mainview when user click on cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Logic for validation process of the creation of a job backup. Check field at every valid button click for security
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnValidClick(object sender, RoutedEventArgs e)
        {
            //Clear errors
            ClearErrorFields();
            //Verify fields and fill errors dictionnary if necessary
            CheckFields();
            //Showing errors if necessary
            if (_createJobViewModel.Errors.Count > 0)
            {
                ShowErrorsFromViewModel();
            }
            else
            {
                int id = -1;
                //Check if job Backup is created
                if (_createJobViewModel.JobBackup != null)
                {
                    id = _createJobViewModel.JobBackup.Id;
                }
                string name = label.Text;
                string sourceDirectory = txtBoxSource.Text;
                string destinationDirectory = txtBoxDestination.Text;
                bool isDifferential = (type.SelectedIndex == 1) ? true : false;
                int priority = comboboxPriority.SelectedIndex;
                _createJobViewModel.JobCreation(id, name, sourceDirectory, destinationDirectory, isDifferential, priority);
                //int priority = _createJobViewModel.GetPriority(txtBoxPriority.Text);
                this.NavigationService.GoBack();
            }

        }

        /// <summary>
        /// Method which called when the focus of label field is lost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelLostFocus(object sender, RoutedEventArgs e)
        {
            //Check and show the error if necessary
            _createJobViewModel.CheckLabel(label.Text);
            if (_createJobViewModel.Errors.ContainsKey("labelError"))
            {
                labelError.Text = _createJobViewModel.Errors["labelError"]; labelError.Visibility = Visibility.Visible;
            }
            else
            {
                labelError.Text = ""; labelError.Visibility = Visibility.Collapsed;
            }
            _createJobViewModel.Errors.Remove("labelError");
        }

        /// <summary>
        /// Clear UI errors
        /// </summary>
        private void ClearErrorFields()
        {
            labelError.Text = ""; labelError.Visibility = Visibility.Collapsed;
            sourceDirectoryError.Text = ""; sourceDirectoryError.Visibility = Visibility.Collapsed;
            destinationDirectoryError.Text = ""; destinationDirectoryError.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Check the fields with view model method
        /// </summary>
        private void CheckFields()
        {
            _createJobViewModel.CheckLabel(label.Text);
            _createJobViewModel.CheckSourceDirectory(txtBoxSource.Text);
            _createJobViewModel.CheckDestinationDirectory(txtBoxDestination.Text, txtBoxSource.Text);
        }

        /// <summary>
        /// Method which read the errors dictionnary of the view model and show it if necessary. Useful for showing error more dynamically to the user.
        /// </summary>
        private void ShowErrorsFromViewModel()
        {
            foreach (KeyValuePair<string, string> entry in _createJobViewModel.Errors)
            {
                switch (entry.Key)
                {
                    case "labelError":
                        labelError.Text = _createJobViewModel.Errors["labelError"]; labelError.Visibility = Visibility.Visible;
                        break;
                    case "sourceDirectoryError":
                        sourceDirectoryError.Text = _createJobViewModel.Errors["sourceDirectoryError"]; sourceDirectoryError.Visibility = Visibility.Visible;
                        break;
                    case "destinationDirectoryError":
                        destinationDirectoryError.Text = _createJobViewModel.Errors["destinationDirectoryError"]; destinationDirectoryError.Visibility = Visibility.Visible;
                        break;
                }
            }
            _createJobViewModel.Errors.Clear();
        }

        /// <summary>
        /// Method which called when the source directory text changed. Useful for showing error more dynamically to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxSourceTextChanged(object sender, TextChangedEventArgs e)
        {
            //Ensure textbox is property loaded
            if (this.IsLoaded)
            {
                //Check and show directory source error
                _createJobViewModel.CheckSourceDirectory(txtBoxSource.Text);
                if (_createJobViewModel.Errors.ContainsKey("sourceDirectoryError"))
                {
                    sourceDirectoryError.Text = _createJobViewModel.Errors["sourceDirectoryError"]; sourceDirectoryError.Visibility = Visibility.Visible;
                }
                else
                {
                    sourceDirectoryError.Text = ""; sourceDirectoryError.Visibility = Visibility.Collapsed;
                }
                _createJobViewModel.Errors.Remove("sourceDirectoryError");
            }

        }

        /// <summary>
        /// Method which called when destination directory text changed. Useful for showing error more dynamically to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxDestinationTextChanged(object sender, TextChangedEventArgs e)
        {
            //Ensure textbox is property loaded
            if (this.IsLoaded)
            {
                //Check and show directory source error
                _createJobViewModel.CheckDestinationDirectory(txtBoxDestination.Text, txtBoxSource.Text);
                if (_createJobViewModel.Errors.ContainsKey("destinationDirectoryError"))
                {
                    destinationDirectoryError.Text = _createJobViewModel.Errors["destinationDirectoryError"]; destinationDirectoryError.Visibility = Visibility.Visible;
                }
                else
                {
                    destinationDirectoryError.Text = ""; destinationDirectoryError.Visibility = Visibility.Collapsed;
                }
                _createJobViewModel.Errors.Remove("destinationDirectoryError");
            }

        }
    }
}