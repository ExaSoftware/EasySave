using EasySave.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasySave
{
    /// <summary>
    /// This class save history and progress logs in this path : 'c:\EasySave\Logs' and create the directory '\Logs if the repository doesn't exist
    /// </summary>
    public class LogModel
    {
        /// <summary>The default directory path for saving logs</summary>
        private const string _DEFAULT_LOG_FILE_PATH = @"C:\EasySave\Logs";

        private const string _DEFAULT_JOB_BACKUP_FILE_PATH = @"C:\EasySave\Job-Backup";

        /// <summary>A list of History log objects which contains all saving action files</summary>
        private List<HistoryLog> myListHistoryLog = new List<HistoryLog>();

        /// <summary>Method which check if the directory exist and create the file if it doesn't exist, 
        /// then deserialized the file if it exist for append the new save job or create a new log file</summary>
        public void SaveHistoryLog(HistoryLog myHistoryLog)
        {
                string path = String.Format(@"{0}\Historylog-{1}.json", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"));
                if (!Directory.Exists(_DEFAULT_LOG_FILE_PATH)) Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);    

                if (File.Exists(path))
                {
                    string myJsonFile = File.ReadAllText(path);
                    var myJsonList = JsonConvert.DeserializeObject<List<HistoryLog>>(myJsonFile);
                    myJsonList.Add(myHistoryLog);
                    string jsonString = JsonConvert.SerializeObject(myJsonList, Formatting.Indented);
                    File.WriteAllText(path, jsonString);
                }

                else
                { 
                    myListHistoryLog.Add(myHistoryLog);
                    string jsonString = JsonConvert.SerializeObject(myListHistoryLog, Formatting.Indented);
                    File.WriteAllText(path, jsonString);
                }
            }

        public void SaveJobBackup(List<JobBackup> jobBackupList)
        {
            string path = String.Format(@"{0}\Save{1}.json", _DEFAULT_JOB_BACKUP_FILE_PATH, "1");
            if (!Directory.Exists(_DEFAULT_JOB_BACKUP_FILE_PATH)) Directory.CreateDirectory(_DEFAULT_JOB_BACKUP_FILE_PATH);
            if (File.Exists(path))
            {
                for (int i = 1; i < 6; i++)
                {
                    string myPath = String.Format(@"{0}\Save{1}.json", _DEFAULT_JOB_BACKUP_FILE_PATH, i.ToString());
                    string myJsonFile = File.ReadAllText(myPath);
                    var myJsonList = JsonConvert.DeserializeObject<List<JobBackup>>(myJsonFile);
                    string jsonString = JsonConvert.SerializeObject(myJsonList, Formatting.Indented);
                    File.WriteAllText(myPath, jsonString);
                }
            }

            else
            {
                for (int i = 1; i < 6; i++)
                {
                    string myPath = String.Format(@"{0}\Save{1}.json", _DEFAULT_JOB_BACKUP_FILE_PATH, i.ToString());
                    string jsonString = JsonConvert.SerializeObject(jobBackupList[i], Formatting.Indented);
                    File.WriteAllText(myPath, jsonString);
                } 
            }
        }

       
       public void SaveProgressLog(List<ProgressLog> progressLogList)
       {
            String path = String.Format(@"{0}\Progresslog.json", _DEFAULT_LOG_FILE_PATH);
            if (!Directory.Exists(_DEFAULT_LOG_FILE_PATH)) Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);
            string json = JsonConvert.SerializeObject(progressLogList, Formatting.Indented);
            File.WriteAllText(path, json);

       }
    }
}