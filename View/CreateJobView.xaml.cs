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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySave
{
    /// <summary>
    /// Logique d'interaction pour CreateJobView.xaml
    /// </summary>
    public partial class CreateJobView : Page
    {
        public CreateJobView()
        {
            InitializeComponent();
        }


        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainView());
        }

        private void btnSelectSourcePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            txtBoxSourcePath.Text = folderBrowserDialog.SelectedPath;
        }

        private void btnSelectDestinationPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            txtBoxDestinationPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainView());
        }

        private void btnValid_Click(object sender, RoutedEventArgs e)
        {
            CreateJobViewModel job = new CreateJobViewModel();
            string name = label.Text;
            string sourceDirectory = txtBoxSourcePath.Text;
            string destinationDirectory = txtBoxDestinationPath.Text;
            bool isDifferential = false;
            if (type.SelectedIndex == 0) isDifferential = false;
            if (type.SelectedIndex == 1) isDifferential = true;
            job.JobCreation(name, sourceDirectory, destinationDirectory, isDifferential);
            this.NavigationService.Navigate(new MainView());
        }
    }
}