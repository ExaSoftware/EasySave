using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace RemoteInterface
{
    public static class Init
    {
        /// <summary>
        /// Load the configuration file of the application
        /// </summary>
        /// <returns>A Configuration Object</returns>
        public static Configuration LoadConfiguration()
        {
            if (!File.Exists(Configuration.DEFAULT_CONFIG_FILE_PATH))
            {
                Configuration configuration = Configuration.GetInstance();
                configuration.CreateConfigurationFile();
                configuration.Language = Thread.CurrentThread.CurrentUICulture.Name;
                configuration.Save();
                return configuration;
            }
            else
            {
                String jsonString = File.ReadAllText(Configuration.DEFAULT_CONFIG_FILE_PATH);
                Configuration configuration = JsonSerializer.Deserialize<Configuration>(jsonString);
                //Set the language according to the language in configuration file
                CultureInfo.CurrentUICulture = new CultureInfo(configuration.Language, false);
                return configuration;
            }
        }
    }
}
