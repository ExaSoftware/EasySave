﻿using EasySave.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
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
    /// Logique d'interaction pour DetailView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        private MainViewModel _mainViewModel;
        private int id;
        /// <summary>
        /// Constructor of the view
        /// </summary>
        public MainView()
        {
            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Method which was executed when a user click on delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteJob_Click(object sender, RoutedEventArgs e)
        {
            if (listViewBackups.SelectedItems.Count != 0)
            {
                _mainViewModel.DeleteSave(id);
                listViewBackups.ItemsSource = null;
                listViewBackups.ItemsSource = _mainViewModel.ListOfJobBackup;
            }
        }
        private void btnAddJob_Click(object sender, RoutedEventArgs e)
        {
            //When we want add a job backup, there is no need to pass a job as parameter
            CreateJobView addJobView = new CreateJobView(new CreateJobViewModel());
            this.NavigationService.Navigate(addJobView);
        }

        /// <summary>
        /// A user click two time on a job backup to show details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Prevent click on the empty list to avoid an exception
            if (listViewBackups.SelectedItems.Count != 0)
            {
                CreateJobView editJobView = new CreateJobView(new CreateJobViewModel((JobBackup)listViewBackups.SelectedItem));
                this.NavigationService.Navigate(editJobView);
            }
        }

        /// <summary>
        /// Method which was excuted when user select a job backup in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewBackups_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Prevent click on the empty list to avoid an exception
            if (listViewBackups.SelectedItems.Count != 0)
            {
                id = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].Id;
                label.Text = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].Label;
                labelSourceDirectory.Text = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].SourceDirectory;
                destinationDirectory.Text = _mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].DestinationDirectory;
                ResourceManager rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
                if (_mainViewModel.ListOfJobBackup[listViewBackups.SelectedIndex].IsDifferential)
                {
                    type.Text = rm.GetString("differential");
                }
                else
                {
                    type.Text = rm.GetString("total");
                }
            }
        }

        /// <summary>
        /// Method which was execute when user click on the button to execute the job backups sequentially
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExecuteSequentially_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ExecuteAll();
        }

        /// <summary>
        /// Method which was executed when user click a the play button to launch a job backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            //If there is a job backup selected
            if (listViewBackups.SelectedItems.Count != 0)
            {
                _mainViewModel.ExecuteOne((JobBackup)listViewBackups.SelectedItem);
            }
            
        }
    }
}
