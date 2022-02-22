using EasySave.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EasySave
{
    /// <summary>
    /// This class save history and progress logs in this path : 'c:\EasySave\Logs' and create the directory '\Logs if the repository doesn't exist
    /// </summary>
    public class JsonReadWriteModel
    {
        /// <summary>The default directory path for saving logs</summary>
        private const string _DEFAULT_LOG_FILE_PATH = @"C:\EasySave\Logs";

        private const string _DEFAULT_JOB_BACKUP_FILE_PATH = @"C:\EasySave\Job-Backup";

        private ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        private ReaderWriterLockSlim _readWriteLockHs = new ReaderWriterLockSlim();


        /// <summary>
        /// Method which check if the directory exist and create the file if it doesn't exist,
        /// then deserialized the file if it exists for appends the new save job or create a new log file
        /// </summary>
        ///<param name=myHistoryLog>An object HistoryLog</param>
        public void SaveHistoryLog(HistoryLog myHistoryLog,int id)
        {
            Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);

            string path = String.Format(@"{0}\Historylog-{1}.{2}", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"), App.Configuration.LogFormat);  //Create the path file

            if (App.Configuration.LogFormat.Equals("json")) SaveHistoryLoginJson(myHistoryLog, path, id);

            else SaveHistoryLogInXml(myHistoryLog, path, id);
        }

        /// <summary>
        /// Delete ProgressLog link to a jobBackup
        /// </summary>
        /// <param name="name"></param>
        public void DeleteProgressLogInJson(string name)
        {
            string path = String.Format(_DEFAULT_LOG_FILE_PATH + @"\ProgressLog.json");
            if (File.Exists(path))
            {
                JObject jsonFile = JObject.Parse(File.ReadAllText(path));
                if (jsonFile.Property(name) != null)
                {
                    File.WriteAllText(path, jsonFile.ToString());
                    jsonFile.Remove(name);
                }
            }
        }

        /// <summary>
        /// Save HistoryLog in xml if the file exists
        /// </summary>
        /// <param name="historyLog"></param>
        /// <param name="path"></param>
        public void SaveHistoryLogInXml(HistoryLog historyLog, string path, int id)
        {
            if (Monitor.TryEnter(path, 2000))
            {
                try
                { 
                    if (File.Exists(path))
                    {
                        using FileStream fsDel = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                        fsDel.SetLength(fsDel.Length - 15);
                        fsDel.Close();

                        Stream xmlFile = new FileStream(path, FileMode.Append, FileAccess.Write);
                        XmlTextWriter xmlwriter = new XmlTextWriter(xmlFile, Encoding.Default)
                        {
                            Formatting = System.Xml.Formatting.Indented
                        };

                        xmlwriter.WriteStartElement("Name");
                        xmlwriter.WriteString( historyLog.Name);
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("SourceFile");
                        xmlwriter.WriteString(historyLog.SourceFile);
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("TargetFile");
                        xmlwriter.WriteString(historyLog.TargetFile);
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("FileSize");
                        xmlwriter.WriteString(historyLog.FileSize.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("TransferTime");
                        xmlwriter.WriteString(historyLog.TransferTime.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("Time");
                        xmlwriter.WriteString(historyLog.Time.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("EncryptionTime");
                        xmlwriter.WriteString(historyLog.EncryptionTime.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("Error");
                        xmlwriter.WriteString(historyLog.Error.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("ErrorTitle");
                        xmlwriter.WriteString(historyLog.ErrorTitle.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.Close();

                        using FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                        using StreamWriter sw = new StreamWriter(fs);
                        sw.WriteLine("\n" + "</HistoryLog>");
                        sw.Close();
                    }

                    else
                    {
                        using StringWriter swHistoryLog = new StringWriter();
                        XmlWriterSettings writtersetting = new XmlWriterSettings();
                        writtersetting.Indent = true;
                        using XmlWriter xmlwriter = XmlWriter.Create(swHistoryLog, writtersetting);

                        xmlwriter.WriteStartDocument();

                        xmlwriter.WriteStartElement("HistoryLog");

                        xmlwriter.WriteStartElement("Name");
                        xmlwriter.WriteString(historyLog.Name);
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("SourceFile");
                        xmlwriter.WriteString(historyLog.SourceFile);
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("TargetFile");
                        xmlwriter.WriteString(historyLog.TargetFile);
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("FileSize");
                        xmlwriter.WriteString(historyLog.FileSize.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("TransferTime");
                        xmlwriter.WriteString(historyLog.TransferTime.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("Time");
                        xmlwriter.WriteString(historyLog.Time.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("EncryptionTime");
                        xmlwriter.WriteString(historyLog.EncryptionTime.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("Error");
                        xmlwriter.WriteString(historyLog.Error.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteStartElement("ErrorTitle");
                        xmlwriter.WriteString(historyLog.ErrorTitle.ToString());
                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteEndElement();

                        xmlwriter.WriteEndDocument();

                        xmlwriter.Close();

                        File.WriteAllText(path, swHistoryLog.ToString() + "\n");
                    }
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
        public void SaveHistoryLoginJson(HistoryLog hs, string path, int id)
        {
            if (Monitor.TryEnter(path, 2000))
            {
                try
                {
                    if (File.Exists(path))
                    {
                        using FileStream fsDel = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                        fsDel.SetLength(fsDel.Length - 1);
                        fsDel.Close();

                        using StringWriter swHistoryLog = new StringWriter();
                        using JsonTextWriter writer = new JsonTextWriter(swHistoryLog);
                        writer.Formatting = Newtonsoft.Json.Formatting.Indented;
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
                        sw.Write("]");
                        sw.Close();
                    }
                    else
                    {
                        using StringWriter sw = new StringWriter();
                        using JsonTextWriter writer = new JsonTextWriter(sw)
                        {
                            Formatting = Newtonsoft.Json.Formatting.Indented
                        };
                        // {
                        writer.WriteStartArray();

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

                        writer.WriteEndArray();

                        writer.Close();

                        File.WriteAllText(path, sw.ToString());
                    }
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
        public void SaveProgressLoginJsonIfFileDoesntExist(ProgressLog pl, string path, int id)
        {
            if (Monitor.TryEnter(path, 2000))
            {
                try
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

                    JObject json = new JObject(new JProperty(pl.Name + " - " + id, newProgressLog));

                    _readWriteLockHs.EnterWriteLock();
                    File.WriteAllText(path, json.ToString());
                    _readWriteLockHs.ExitWriteLock();
                }
                finally
                {
                    Monitor.Exit(path);
                }
            }
        }

        /// <summary>
        /// Save progress log if file exists
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="path"></param>
        public void SaveProgressLoginJsonIfFileExist(ProgressLog pl, string path, int id)
        {
            JObject jsonFile = null;
            if (Monitor.TryEnter(path, 2000))
            {
                _readWriteLockHs.EnterReadLock();
                try
                {
                    jsonFile = JObject.Parse(File.ReadAllText(path));
                }
                finally
                {
                    _readWriteLockHs.ExitReadLock();
                    Monitor.Exit(path);
                }

                if (jsonFile.Property(pl.Name + " - " + id) != null)
                {
                    JObject progressLogToUpdate = (JObject)jsonFile[pl.Name + " - " + id];
                    progressLogToUpdate["SourceFile"] = pl.SourceFile;
                    progressLogToUpdate["TargetFile"] = pl.TargetFile;
                    progressLogToUpdate["State"] = pl.State;
                    progressLogToUpdate["TotalFilesToCopy"] = pl.TotalFilesToCopy;
                    progressLogToUpdate["TotalFilesSize"] = pl.TotalFilesSize;
                    progressLogToUpdate["TotalFilesRemaining"] = pl.TotalFilesRemaining;
                    progressLogToUpdate["Progression"] = pl.Progression;

                    if (Monitor.TryEnter(path, 3000))
                    {
                        _readWriteLockHs.EnterWriteLock();
                        try
                        {
                            File.WriteAllText(path, jsonFile.ToString());
                        }
                        catch
                        {

                        }

                        finally
                        {
                            _readWriteLockHs.ExitWriteLock();
                            Monitor.Exit(path);
                        }
                    }
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

                    jsonFile.Add(new JProperty(pl.Name + " - " + id, newProgressLog));

                    if (Monitor.TryEnter(path, 3000))
                    {
                        try
                        {
                            File.WriteAllText(path, jsonFile.ToString());
                        }
                        finally
                        {
                            Monitor.Exit(path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method which read a json file and convert it into a list of list of JobBackup
        /// </summary>
        /// <returns>The list of JobBackup associate to the json file</returns>
        public ObservableCollection<JobBackup> ReadJobBackup()
        {
            string path = String.Format(@"{0}\SavedJobBackup.json", _DEFAULT_JOB_BACKUP_FILE_PATH);
            if (Monitor.TryEnter(path, 10000))
            {
                ObservableCollection<JobBackup> listOfJobackup = new ObservableCollection<JobBackup>();
                try
                {
                    if (File.Exists(path))
                    {
                        string myJsonFile = File.ReadAllText(path);
                        listOfJobackup = JsonConvert.DeserializeObject<ObservableCollection<JobBackup>>(myJsonFile);
                    }
                    else
                    {
                        File.Create(path).Close();
                    }
                }

                finally
                {
                    Monitor.Exit(path);
                }
                return listOfJobackup;
            }
            return ReadJobBackup();
        }

        /// <summary>
        /// Method which save a list of jobBackup into a json file
        /// </summary>
        /// <param name="jobBackupList"></param>
        public void SaveJobBackup(ObservableCollection<JobBackup> jobBackupList)
        {

            Directory.CreateDirectory(_DEFAULT_JOB_BACKUP_FILE_PATH);
            string jsonStringJobBackup = JsonConvert.SerializeObject(jobBackupList, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(String.Format(@"{0}\SavedJobBackup.json", _DEFAULT_JOB_BACKUP_FILE_PATH), jsonStringJobBackup);
        }

        /// <summary>
        /// Method which save a list of progressLog into a json file
        /// </summary>
        /// <param name="progressLogList"></param>
        public void SaveProgressLog(ProgressLog myProgressLog, int id)
        {
            String path = String.Format(@"{0}\Progresslog.json", _DEFAULT_LOG_FILE_PATH);
            Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);
            if (File.Exists(path))
            {
                SaveProgressLoginJsonIfFileExist(myProgressLog, path, id);
            }
            else
            {
                SaveProgressLoginJsonIfFileDoesntExist(myProgressLog, path, id);
            }
        }

    }
}