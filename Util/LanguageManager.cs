using System;
using System.Globalization;

namespace EasySave.Util
{
    public static class LanguageManager
    {
        public static void ChangeLanguage(String language)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language, false);
        }
    }
}
