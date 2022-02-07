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
        private static SettingsView _instance;
        public SettingsView()
        {
            InitializeComponent();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new DetailView());
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
