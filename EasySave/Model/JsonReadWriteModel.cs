using EasySave.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

            if (File.Exists(path))
            {
                //if the format is JSON and the file already exists
                if (Path.GetExtension(path) == ".json")
                {

                    SaveinJsonIfFileExist(myHistoryLog, path);
                }

                else
                {

                    SaveXmlIfFileExists(myHistoryLog, path);
                }
            }

            else
            {

                if (App.Configuration.LogFormat.Equals("json"))
                {

                    SaveinJsonIfFileDoesntExist(myHistoryLog, path);
                }

                else
                {
                    SaveXmlIfFileDoesntExists(myHistoryLog, path);
                }
            }
        }

        /// <summary>
        /// Save HistoryLog in xml if the file doesn't exists
        /// </summary>
        /// <param name="historyLog"></param>
        /// <param name="path"></param>
        public static void SaveXmlIfFileDoesntExists(HistoryLog historyLog, string path)
        {
            XDocument newHistoryLog = new XDocument(

                new XElement("ArrayOfHistoryLog",
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
                )));

            newHistoryLog.Save(path);
        }

        /// <summary>
        /// Save HistoryLog in xml if the file exists
        /// </summary>
        /// <param name="historyLog"></param>
        /// <param name="path"></param>
        public static void SaveXmlIfFileExists(HistoryLog historyLog, string path)
        {
            if (Monitor.TryEnter(path, 2000))
            {
                try
                {
                    XDocument doc = XDocument.Load(path);
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
                    doc.Root.Add(newHistoryLog);
                    doc.Save(path);
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
        public static void SaveinJsonIfFileDoesntExist(this HistoryLog hs, string path)
        {
            using StringWriter sw = new StringWriter();
            using JsonTextWriter writer = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented
            };
            // {

            writer.WriteStartObject();

            // "name" : "Save"
            writer.WritePropertyName("Name");
            writer.WriteValue(hs.Name);
            writer.WritePropertyName("SourceFile");
            writer.WriteValue(hs.SourceFile);
            writer.WritePropertyName("TargetFile");
            writer.WriteValue(hs.TargetFile);
            writer.WritePropertyName("FileSize");
            writer.WriteValue(hs.FileSize);
            writer.WritePropertyName("TransferTime");
            writer.WriteValue(hs.TransferTime);
            writer.WritePropertyName("Time");
            writer.WriteValue(hs.Time);
            writer.WritePropertyName("EncryptionTime");
            writer.WriteValue(hs.EncryptionTime);
            writer.WritePropertyName("Error");
            writer.WriteValue(hs.Error);
            writer.WritePropertyName("ErrorTitle");
            writer.WriteValue(hs.ErrorTitle);
            // }
            writer.WriteEndObject();

            writer.Close();

            File.WriteAllTextAsync(path, sw.ToString());
        }

        /// <summary>
        /// Save HistoryLog in Json if the file exists
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="path"></param>
        public static void SaveinJsonIfFileExist(this HistoryLog hs, string path)
        {
            if (Monitor.TryEnter(path, 2000))
            {
                try
                {
                    using StringWriter swHistoryLog = new StringWriter();
                    using JsonTextWriter writer = new JsonTextWriter(swHistoryLog);
                    writer.Formatting = Formatting.Indented;
                    // {
                    writer.WriteStartObject();
                    // "name" : "Save"
                    writer.WritePropertyName("Name");
                    writer.WriteValue(hs.Name);
                    writer.WritePropertyName("SourceFile");
                    writer.WriteValue(hs.SourceFile);
                    writer.WritePropertyName("TargetFile");
                    writer.WriteValue(hs.TargetFile);
                    writer.WritePropertyName("FileSize");
                    writer.WriteValue(hs.FileSize);
                    writer.WritePropertyName("TransferTime");
                    writer.WriteValue(hs.TransferTime);
                    writer.WritePropertyName("Time");
                    writer.WriteValue(hs.Time);
                    writer.WritePropertyName("EncryptionTime");
                    writer.WriteValue(hs.EncryptionTime);
                    writer.WritePropertyName("Error");
                    writer.WriteValue(hs.Error);
                    writer.WritePropertyName("ErrorTitle");
                    writer.WriteValue(hs.ErrorTitle);

                    // }
                    writer.WriteEndObject();
                    writer.Close();

                    string jsonHistoryLog = swHistoryLog.ToString();
                    using FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                    using StreamWriter sw = new StreamWriter(fs);
                    sw.Write(",");
                    sw.WriteLine(jsonHistoryLog);
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
        public static void SaveProgressLoginJsonIfFileDoesntExist(this ProgressLog pl, string path)
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

                        using StringWriter swHistoryLog = new StringWriter();
                        using JsonTextWriter writer = new JsonTextWriter(swHistoryLog);
                        writer.Formatting = Formatting.Indented;
                        // {
                        writer.WriteStartObject();
                        // "name" : "Save"
                        writer.WritePropertyName("Name");
                        writer.WriteValue(pl.Name);
                        writer.WritePropertyName("SourceFile");
                        writer.WriteValue(pl.SourceFile);
                        writer.WritePropertyName("TargetFile");
                        writer.WriteValue(pl.TargetFile);
                        writer.WritePropertyName("State");
                        writer.WriteValue(pl.State);
                        writer.WritePropertyName("TotalFilesToCopy");
                        writer.WriteValue(pl.TotalFilesToCopy);
                        writer.WritePropertyName("TotalFilesSize");
                        writer.WriteValue(pl.TotalFilesSize);
                        writer.WritePropertyName("TotalFilesRemaining");
                        writer.WriteValue(pl.TotalFilesRemaining);
                        writer.WritePropertyName("Progression");
                        writer.WriteValue(pl.Progression);
                        // }
                        writer.WriteEndObject();
                        writer.Close();

                        string jsonHistoryLog = swHistoryLog.ToString();
                        using FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                        using StreamWriter sw = new StreamWriter(fs);
                        sw.Write(",");
                        sw.WriteLine(jsonHistoryLog);
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
        public static List<JobBackup> ReadJobBackup()
        {
            string path = String.Format(@"{0}\SavedJobBackup.json", _DEFAULT_JOB_BACKUP_FILE_PATH);
            if (File.Exists(path))
            {
                string myJsonFile = File.ReadAllText(path);
                var JobBackupJsonList = JsonConvert.DeserializeObject<List<JobBackup>>(myJsonFile);
                if (JobBackupJsonList == null) return new List<JobBackup>();
                return JobBackupJsonList;
            }
            else
            {
                File.Create(path).Close();
                return new List<JobBackup>();
            }
        }

        /// <summary>
        /// Method which save a list of jobBackup into a json file
        /// </summary>
        /// <param name="jobBackupList"></param>
        public static void SaveJobBackup(List<JobBackup> jobBackupList)
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