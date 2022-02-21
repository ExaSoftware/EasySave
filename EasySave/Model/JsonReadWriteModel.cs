using EasySave.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;

namespace EasySave
{
    /// <summary>
    /// This class save history and progress logs in this path : 'c:\EasySave\Logs' and create the directory '\Logs if the repository doesn't exist
    /// </summary>
    public static class JsonReadWriteModel
    {
        /// <summary>The default directory path for saving logs</summary>
        private const string _DEFAULT_LOG_FILE_PATH = @"C:\EasySave\Logs";

        private const string _DEFAULT_JOB_BACKUP_FILE_PATH = @"C:\EasySave\Job-Backup";

        /// <summary>
        /// Method which check if the directory exist and create the file if it doesn't exist, 
        /// then deserialized the file if it exists for appends the new save job or create a new log file
        /// </summary>
        ///<param name=myHistoryLog>An object HistoryLog</param>
        public static void SaveHistoryLog(HistoryLog myHistoryLog)
        {
            Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);

            string path = String.Format(@"{0}\Historylog-{1}.{2}", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"), App.Configuration.LogFormat);  //Create the path file

            if (App.Configuration.LogFormat.Equals("json")) SaveHistoryLoginJson(myHistoryLog, path);

            else SaveHistoryLogInXml(myHistoryLog, path);
        }

        /// <summary>
        /// Delete ProgressLog link to a jobBackup
        /// </summary>
        /// <param name="name"></param>
        public static void DeleteProgressLogInJson(string name)
        {
            string path = String.Format(_DEFAULT_LOG_FILE_PATH + @"\ProgressLog.json");
            if (File.Exists(path))
            {
                JObject jsonFile = JObject.Parse(File.ReadAllText(path));
                bool res = jsonFile.Remove(name);
                File.WriteAllTextAsync(path, jsonFile.ToString());
            }  
        }

        /// <summary>
        /// Save HistoryLog in xml if the file exists
        /// </summary>
        /// <param name="historyLog"></param>
        /// <param name="path"></param>
        public static void SaveHistoryLogInXml(HistoryLog historyLog, string path)
        {
            if (Monitor.TryEnter(path, 2000))
            {
                try
                {
                    XElement newHistoryLog =
                    new XElement("HistoryLog",
                    new XElement("Name", historyLog.Name),
                    new XElement("SourceFile", historyLog.SourceFile),
                    new XElement("TargetFile", historyLog.TargetFile),
                    new XElement("FileSize", historyLog.FileSize.ToString()),
                    new XElement("TransferTime", historyLog.TransferTime.ToString()),
                    new XElement("Time", historyLog.Time.ToString()),
                    new XElement("EncryptionTime", historyLog.EncryptionTime.ToString()),
                    new XElement("Error", historyLog.Error),
                    new XElement("ErrorTitle", historyLog.ErrorTitle)
                    );
                    XDocument doc;
                    if (File.Exists(path))
                    {
                        doc = XDocument.Load(path);
                        doc.Root.Add(newHistoryLog);
                        doc.Save(path);
                    }

                    else newHistoryLog.Save(path);

                }
                finally
                {
                    Monitor.Exit(path);
                }
            }
        }

        /// <summary>
        /// Save HistoryLog in Json in the file doesn't exists
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="path"></param>
        public static void SaveHistoryLoginJson(HistoryLog hs, string path)
        {
            JObject newHistoryLog = new JObject(

            new JProperty("Name", hs.Name),
            new JProperty("SourceFile", hs.SourceFile),
            new JProperty("TargetFile", hs.TargetFile),
            new JProperty("FileSize", hs.FileSize),
            new JProperty("TransferTime", hs.TransferTime),
            new JProperty("Time", hs.Time),
            new JProperty("EncryptionTime", hs.EncryptionTime),
            new JProperty("Error", hs.Error),
            new JProperty("ErrorTitle", hs.ErrorTitle)
            );
            JObject json;
            if (Monitor.TryEnter(path, 2000))
            {
                try
                {
                    if (File.Exists(path))
                    {
                        json = JObject.Parse(File.ReadAllText(path));

                        json.Add(new JProperty(hs.Name + " - " + Path.GetFileName(hs.SourceFile) +" - " + hs.Time + " - " + hs.TransferTime,newHistoryLog));
                    }
                    else json = new JObject(new JProperty(hs.Name + " - " + Path.GetFileName(hs.SourceFile) + " - " + hs.Time + " - " + hs.TransferTime, newHistoryLog));

                    File.WriteAllTextAsync(path, json.ToString());
                }
                finally
                {
                    Monitor.Exit(path);
                }
            }
        }

        /// <summary>
        /// Save progress log in json if the file doesn't exists
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="path"></param>
        public static void SaveProgressLoginJsonIfFileDoesntExist(ProgressLog pl, string path)
        {
            JObject newProgressLog = new JObject(

            new JProperty("Name", pl.Name),
            new JProperty("SourceFile", pl.SourceFile),
            new JProperty("TargetFile", pl.TargetFile),
            new JProperty("State", pl.State),
            new JProperty("TotalFilesToCopy", pl.TotalFilesToCopy),
            new JProperty("TotalFilesSize", pl.TotalFilesRemaining),
            new JProperty("TotalFilesRemaining", pl.TotalFilesRemaining),
            new JProperty("Progression", pl.Progression)
            );

            JObject json = new JObject(new JProperty(pl.Name, newProgressLog));

            File.WriteAllTextAsync(path, json.ToString());
        }

        /// <summary>
        /// Save progress log if file exists
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="path"></param>
        public static void SaveProgressLoginJsonIfFileExist(ProgressLog pl, string path)
        {

            if (Monitor.TryEnter(path, 2000))
            {
                try
                {
                    JObject jsonFile = JObject.Parse(File.ReadAllText(path));

                    if (jsonFile.Property(pl.Name) != null)
                    {
                        JObject progressLogToUpdate = (JObject)jsonFile[pl.Name];
                        progressLogToUpdate["SourceFile"] = pl.SourceFile;
                        progressLogToUpdate["TargetFile"] = pl.TargetFile;
                        progressLogToUpdate["State"] = pl.State;
                        progressLogToUpdate["TotalFilesToCopy"] = pl.TotalFilesToCopy;
                        progressLogToUpdate["TotalFilesSize"] = pl.TotalFilesSize;
                        progressLogToUpdate["TotalFilesRemaining"] = pl.TotalFilesRemaining;
                        progressLogToUpdate["Progression"] = pl.Progression;
                        File.WriteAllTextAsync(path, jsonFile.ToString());
                    }
                    else
                    {
                        JObject newProgressLog = new JObject(

                        new JProperty("Name", pl.Name),
                        new JProperty("SourceFile", pl.SourceFile),
                        new JProperty("TargetFile", pl.TargetFile),
                        new JProperty("State", pl.State),
                        new JProperty("TotalFilesToCopy", pl.TotalFilesToCopy),
                        new JProperty("TotalFilesSize", pl.TotalFilesRemaining),
                        new JProperty("TotalFilesRemaining", pl.TotalFilesRemaining),
                        new JProperty("Progression", pl.Progression)
                        );

                        jsonFile.Add(new JProperty(pl.Name, newProgressLog));

                        File.WriteAllTextAsync(path, jsonFile.ToString());
                    }
                }
                finally
                {
                    Monitor.Exit(path);
                }
            }
        }

        /// <summary>
        /// Delete a file and wait if the file is open
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            while (true)
            {
                try
                {
                    File.Delete(path);
                    break;
                }

                catch (IOException)
                {
                    continue;
                }

                catch (Exception e)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Method which read a json file and convert it into a list of list of JobBackup
        /// </summary>
        /// <returns>The list of JobBackup associate to the json file</returns>
        public static ObservableCollection<JobBackup> ReadJobBackup()
        {
            string path = String.Format(@"{0}\SavedJobBackup.json", _DEFAULT_JOB_BACKUP_FILE_PATH);
            if (File.Exists(path))
            {
                string myJsonFile = File.ReadAllText(path);
                var JobBackupJsonList = JsonConvert.DeserializeObject<ObservableCollection<JobBackup>>(myJsonFile);
                if (JobBackupJsonList == null) return new ObservableCollection<JobBackup>();
                return JobBackupJsonList;
            }
            else
            {
                File.Create(path).Close();
                return new ObservableCollection<JobBackup>();
            }
        }

        /// <summary>
        /// Method which save a list of jobBackup into a json file
        /// </summary>
        /// <param name="jobBackupList"></param>
        public static void SaveJobBackup(ObservableCollection<JobBackup> jobBackupList)
        {
            Directory.CreateDirectory(_DEFAULT_JOB_BACKUP_FILE_PATH);
            string jsonStringJobBackup = JsonConvert.SerializeObject(jobBackupList, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllTextAsync(String.Format(@"{0}\SavedJobBackup.json", _DEFAULT_JOB_BACKUP_FILE_PATH), jsonStringJobBackup);
        }

        /// <summary>
        /// Method which save a list of progressLog into a json file
        /// </summary>
        /// <param name="progressLogList"></param>
        public static void SaveProgressLog(ProgressLog myProgressLog)
        {
            String path = String.Format(@"{0}\Progresslog.json", _DEFAULT_LOG_FILE_PATH);
            Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);
            if (File.Exists(path))
            {
                SaveProgressLoginJsonIfFileExist(myProgressLog, path);
            }
            else
            {
                SaveProgressLoginJsonIfFileDoesntExist(myProgressLog, path);
            }
        }

    }
}