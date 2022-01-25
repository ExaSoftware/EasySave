using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EasySave
{
    public class LogModel
    {
        private const string _DEFAULTLOGFILEPATH = @"C:\Logs";
        public bool SaveHistoryLog(HistoryLog myHistoryLog)
        {
            try
            {
                if (!File.Exists(_DEFAULTLOGFILEPATH)) File.Create(_DEFAULTLOGFILEPATH);

                string jsonString = JsonSerializer.Serialize(myHistoryLog);

                File.WriteAllText(@"c:\EasySave\Logs", jsonString);
            }

            catch (Exception e)
            {
                //if (e is DirectoryNotFoundException) System.IO.Directory.CreateDirectory(_logFilePath);
                if (e is FileNotFoundException)
                {
                    Console.WriteLine("The specific file doesn't exist");
                    return false;
                }
            }
            return true;
        }

        /*
        public bool SaveProgressLog()
        {

        }*/
    }


}
