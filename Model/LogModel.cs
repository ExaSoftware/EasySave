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
        /// then deserialized the file if it exist for append the new save job or create a new log file
        /// </summary>
        ///<param name=myHistoryLog>An object HistoryLog</param>
        public void SaveHistoryLog(HistoryLog myHistoryLog)
        {
                string path = String.Format(@"{0}\Historylog-{1}.json", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"));    //Create the path file
                if (!Directory.Exists(_DEFAULT_LOG_FILE_PATH)) Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);                       //Check that the directory exist and create it if it doesn't exist

                if (File.Exists(path))
                {
                    string myJsonFile = File.ReadAllText(path);                                         //Get the right file 
                    var myJsonList = JsonConvert.DeserializeObject<List<HistoryLog>>(myJsonFile);       //Convert the Json file into a list of HistoryLog
                    myJsonList.Add(myHistoryLog);                                                       //Append the parameter in the list which have been retrieve from the json file
                    string jsonString = JsonConvert.SerializeObject(myJsonList, Formatting.Indented);   //Serialize the list of HistoryLog into a json file
                    File.WriteAllText(path, jsonString);                                                //Over write the file with the new object
                }

                else
                { 
                    myListHistoryLog.Add(myHistoryLog);                                                         //if the file doesn't exist, append the new HistoryLog in the empty list of HistoryLog
                    string jsonString = JsonConvert.SerializeObject(myListHistoryLog, Formatting.Indented);     //Serialize the list of HistoryLog into a json file
                    File.WriteAllText(path, jsonString);                                                        //Write the file with the object HistoryLog
                }
            }

        /// <summary>Method which read a json file and convert it into a list of list of JobBackup</summary>
        /// <returns>The list of JobBackup associate to the json file</returns>
        public List<JobBackup> ReadJobBackup()
        {
            string path = String.Format(@"{0}\SavedJobBackup.json", _DEFAULT_JOB_BACKUP_FILE_PATH);          
            if (File.Exists(path))
            {
                    string myJsonFile = File.ReadAllText(path);
                    var JobBackupJsonList = JsonConvert.DeserializeObject<List<JobBackup>>(myJsonFile);
                    return JobBackupJsonList;
            }
            else
            {
                return null;
            }
        }
      
        /// <summary>
        /// Method which save a list of jobBackup into a json file
        /// </summary>
        /// <param name="jobBackupList"></param>
        public void SaveJobBackup(List<JobBackup> jobBackupList)
        {
            if (!Directory.Exists(_DEFAULT_JOB_BACKUP_FILE_PATH)) Directory.CreateDirectory(_DEFAULT_JOB_BACKUP_FILE_PATH);
            string jsonStringJobBackup = JsonConvert.SerializeObject(jobBackupList, Formatting.Indented);
            File.WriteAllText(String.Format(@"{0}\SavedJobBackup.json", _DEFAULT_JOB_BACKUP_FILE_PATH), jsonStringJobBackup);
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