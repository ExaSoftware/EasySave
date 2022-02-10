using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Configuration _configuration;
        App()
        {
            Init.CreateDataDirectoryIfNotExists();
            _configuration = Init.LoadConfiguration();
            //Debug.WriteLine(CultureInfo.CurrentUICulture);
        }

        public static Configuration Configuration { get => _configuration; }
    }
}
