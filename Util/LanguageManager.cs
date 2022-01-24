using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace EasySave.Util
{
    public static class LanguageManager
    {
        public static void ChangeLanguage(String language)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
        }
    }
}
