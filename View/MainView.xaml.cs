﻿using EasySave.ViewModel;
using System;
using System.Collections.Generic;
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
        private MainViewModel _detailViewModel;
        public MainView()
        {
            _detailViewModel = new MainViewModel();
            this.DataContext = _detailViewModel;
            InitializeComponent();
        }

        private void btnAddJob_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CreateJobView());
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CreateJobView editJobView = new CreateJobView();
            editJobView.DataContext = new CreateJobViewModel((JobBackup)listViewBackups.SelectedItem);
            this.NavigationService.Navigate(editJobView);
        }
    }
}