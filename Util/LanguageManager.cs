using EasySave.Object;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace EasySave.Util
{
    public static class LanguageManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="language">The want to setup</param>
        /// <param name="configuration">The configuration object</param>
        public static void ChangeLanguage(String language, Configuration configuration)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language, false);
            configuration.Language = language;
            configuration.Save();
            /*Thread.Sleep(2000);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();*/
        }
    }
}
