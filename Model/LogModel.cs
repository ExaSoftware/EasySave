using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EasySave
{
    /// <summary>
    /// This class save history and progress logs in this path : 'c:\EasySave\Logs' and create the directory '\Logs if the repository doesn't exist
    /// </summary>
    public class LogModel
    {
        /// <summary>The default directory path for saving logs</summary>
        private const string _DEFAULTLOGFILEPATH = @"C:\EasySave\Logs";

        /// <summary>A list of History log objects which contains all saving action files</summary>
        private List<HistoryLog> myListHistoryLog = new List<HistoryLog>();

        /// <summary>Method which check if the directory exist and create the file if it doesn't exist, 
        /// then deserialized the file if it exist for append the new save job or create a new log file</summary>
        public void SaveHistoryLog(HistoryLog myHistoryLog)
        {
                if (!Directory.Exists(_DEFAULTLOGFILEPATH)) Directory.CreateDirectory(_DEFAULTLOGFILEPATH);

                if (File.Exists(String.Format(@"{0}\Historylog-{1}.json", _DEFAULTLOGFILEPATH, DateTime.Now.ToString("d-MM-yyyy")))) {
                    
                    string myJsonFile = File.ReadAllText(String.Format(@"{0}\Historylog-{1}.json", _DEFAULTLOGFILEPATH, DateTime.Now.ToString("d-MM-yyyy")));
                    var  myJsonList  = JsonConvert.DeserializeObject<List<HistoryLog>>(myJsonFile);
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    myJsonList.Add(myHistoryLog);
                    string jsonString = JsonConvert.SerializeObject(myJsonList, Formatting.Indented);
                    File.WriteAllText(String.Format(@"{0}\Historylog-{1}.json", _DEFAULTLOGFILEPATH, DateTime.Now.ToString("d-MM-yyyy")), jsonString);
                }

                else
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    myListHistoryLog.Add(myHistoryLog);
                    string jsonString = JsonConvert.SerializeObject(myListHistoryLog, Formatting.Indented);
                    Console.WriteLine(jsonString);
                    File.WriteAllText(String.Format(@"{0}\Historylog-{1}.json", _DEFAULTLOGFILEPATH, DateTime.Now.ToString("d-MM-yyyy")), jsonString);
                }
            }
                /*
               public bool SaveProgressLog()
               {

               }*/
    }
}