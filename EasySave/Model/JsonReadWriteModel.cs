using EasySave.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

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
        public void SaveHistoryLog(HistoryLog myHistoryLog, int id)
        {
            _ = Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);

            string path = String.Format(@"{0}\Historylog-{1}.{2}", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"), App.Configuration.LogFormat);  //Create the path file

            if (App.Configuration.LogFormat.Equals("json")) SaveHistoryLoginJson(myHistoryLog, path, id);

            else SaveHistoryLogInXml(myHistoryLog, path, id);
        }

        /// <summary>
        /// Method which save a list of progressLog into a json file
        /// </summary>
        /// <param name="progressLogList"></param>
        public void SaveProgressLog(ProgressLog myProgressLog, int id)
        {
            string path = String.Format(@"{0}\Progresslog.json", _DEFAULT_LOG_FILE_PATH);
            _ = Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);
            if (File.Exists(path))
            {
                SaveProgressLoginJsonIfFileExist(myProgressLog, path, id);
            }
            else
            {
                SaveProgressLoginJsonIfFileDoesntExist(myProgressLog, path, id);
            }
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
                    _ = jsonFile.Remove(name);
                }
            }
        }

        /// <summary>
        /// Save HistoryLog in xml if the file exists
        /// </summary>
        /// <param name="historyLog"></param>
        /// <param name="path"></param>
        private void SaveHistoryLogInXml(HistoryLog historyLog, string path, int id)
        {
            if (Monitor.TryEnter(path, 2000))
            {
                try
                {
                    if (File.Exists(path))
                    {
                        while (true)
                        {
                            try
                            {
                                using FileStream fsDel = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                                fsDel.SetLength(fsDel.Length - 15);
                                fsDel.Close();
                                fsDel.Dispose();
                                break;
                            }
                            catch (IOException)
                            {
                                continue;
                            }
                        }

                        while (true)
                        {
                            try
                            {
                                using Stream xmlFile = new FileStream(path, FileMode.Append, FileAccess.Write);
                                using XmlTextWriter xmlwriter = new XmlTextWriter(xmlFile, Encoding.Default)
                                {
                                    Formatting = System.Xml.Formatting.Indented
                                };

                                xmlwriter.WriteStartElement("Name" + " - " + id);
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

                                xmlwriter.Close();
                                break;
                            }
                            catch (IOException)
                            {
                                continue;
                            }

                        }

                        while (true)
                        {
                            try
                            {
                                using FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                                using StreamWriter sw = new StreamWriter(fs);
                                sw.WriteLine("\n" + "</HistoryLog>");
                                sw.Close();
                                break;
                            }
                            catch (IOException)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            try
                            {
                                using Stream xmlFile = new FileStream(path, FileMode.Create, FileAccess.Write);
                                using XmlTextWriter xmlwriter = new XmlTextWriter(xmlFile, Encoding.Default)
                                {
                                    Formatting = System.Xml.Formatting.Indented
                                };

                                xmlwriter.WriteStartDocument();

                                xmlwriter.WriteStartElement("HistoryLog");

                                xmlwriter.WriteStartElement("Name" + " - " + id);
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
                                break;
                            }
                            catch (IOException)
                            {
                                continue;
                            }
                        }
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
        private void SaveHistoryLoginJson(HistoryLog hs, string path, int id)
        {
            if (Monitor.TryEnter(path, 2000))
            {
                try
                {
                    if (File.Exists(path))
                    {
                        while (true)
                        {
                            try
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
                                writer.WritePropertyName("Name" + " - " + id);
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
                                break;
                            }
                            catch (IOException)
                            {
                                Thread.Sleep(10);
                                continue;
                            }
                        }
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
                        writer.WritePropertyName("Name" + " - " + id);
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
                        _readWriteLockHs.EnterWriteLock();
                        File.WriteAllText(path, sw.ToString());
                        _readWriteLockHs.ExitWriteLock();
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
        private void SaveProgressLoginJsonIfFileDoesntExist(ProgressLog pl, string path, int id)
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

                    _readWriteLock.EnterWriteLock();
                    File.WriteAllText(path, json.ToString());
                    _readWriteLock.ExitWriteLock();
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
        private void SaveProgressLoginJsonIfFileExist(ProgressLog pl, string path, int id)
        {
            while (true)
            {
                try
                {
                    using FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    JObject jsonFile = null;
                    jsonFile = JObject.Parse(ReadText(fileStream));

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

                        fileStream.SetLength(0);
                        StreamWriter stream = new StreamWriter(fileStream);
                        stream.Write(jsonFile.ToString());

                        stream.Close();
                        fileStream.Close();
                        stream.Dispose();
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

                        fileStream.SetLength(0);
                        StreamWriter stream = new StreamWriter(fileStream);
                        stream.WriteLine(jsonFile.ToString());

                        stream.Close();
                        fileStream.Close();
                        stream.Dispose();
                    }
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(10);
                    continue;
                }
            }
        }

        /// <summary>
        /// Read all the text in the fileStream.
        /// </summary>
        /// <param name="stream">The fileStream to read</param>
        /// <returns>The content of the file i a string.</returns>
        private string ReadText(FileStream stream)
        {
            int totalBytes = (int)stream.Length;
            byte[] bytes = new byte[totalBytes];
            int bytesRead = 0;

            while (bytesRead < totalBytes)
            {
                int len = stream.Read(bytes, bytesRead, totalBytes);
                bytesRead += len;
            }

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Method which read a json file and convert it into a list of list of JobBackup.
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
    }
}