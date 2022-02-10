using EasySave.ViewModel;
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
        private MainViewModel _mainViewModel;
        public MainView()
        {
            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
            InitializeComponent();
        }
        private void btnDeleteJob_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel view = new MainViewModel();
            view.DeleteSave();
            this.NavigationService.Navigate(new MainView());
        }
        private void btnAddJob_Click(object sender, RoutedEventArgs e)
        {
            //When we want add a job backup, there is no need to pass a job as parameter
            CreateJobView addJobView = new CreateJobView(new CreateJobViewModel());
            this.NavigationService.Navigate(addJobView);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CreateJobView editJobView = new CreateJobView(new CreateJobViewModel((JobBackup)listViewBackups.SelectedItem));
            this.NavigationService.Navigate(editJobView);
        }
    }
}
