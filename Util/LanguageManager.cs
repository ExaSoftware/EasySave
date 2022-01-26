using EasySave.Object;
using System;
using System.Globalization;

namespace EasySave.Util
{
    public static class LanguageManager
    {
        public static void ChangeLanguage(String language, Configuration configuration)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language, false);
            configuration.Language = language;
            configuration.Save();
        }
    }
}
