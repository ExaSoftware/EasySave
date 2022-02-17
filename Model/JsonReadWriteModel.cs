using EasySave.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

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

        /// <summary>A list of History log objects which contains all saving action files</summary>
        private List<HistoryLog> _myListHistoryLog = new List<HistoryLog>();

        /// <summary>A list of Progress log objects which contains all saving action files</summary>
        private List<ProgressLog> _myProgressLogList = new List<ProgressLog>();

        private Configuration _configuration = Init.LoadConfiguration();

        /// <summary>Method which save an HistoryLog in json or xml format depending of user's choice
        /// </summary>
        ///<param name=myHistoryLog>An object HistoryLog</param>
        public void SaveHistoryLog(HistoryLog myHistoryLog)
        {
            Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);
           
            string path = String.Format(@"{0}\Historylog-{1}.{2}", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"), _configuration.LogFormat);  //Create the path file
           
            if (File.Exists(path))
            {
                string file = File.ReadAllText(path);

                if (Path.GetExtension(path) == ".json")
                {
                    string xmlPath = String.Format(@"{0}\Historylog-{1}.xml", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"));
                    if (File.Exists(xmlPath))
                    {
                        this.GetListFromXml(xmlPath);
                        _myListHistoryLog.Add(myHistoryLog);
                    }

                    else
                    {
                        _myListHistoryLog = JsonConvert.DeserializeObject<List<HistoryLog>>(file);                             //Convert the Json file into a list of HistoryLog
                        _myListHistoryLog.Add(myHistoryLog);
                        this.DeleteFile(xmlPath);
                    }
                }

                else
                {
                    string jsonPath = String.Format(@"{0}\Historylog-{1}.json", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"));

                    if (File.Exists(jsonPath))
                    {
                        this.GetListFromJson(jsonPath);
                        _myListHistoryLog.Add(myHistoryLog);
                    }

                    else
                    {
                        var serializer = new XmlSerializer(typeof(List<HistoryLog>));
                        using TextReader reader = new StringReader(file);
                        _myListHistoryLog = (List<HistoryLog>)serializer.Deserialize(reader);
                        _myListHistoryLog.Add(myHistoryLog);
                    }
                }

                string jsonString = JsonConvert.SerializeObject(_myListHistoryLog, Newtonsoft.Json.Formatting.Indented);

                if (_configuration.LogFormat.Equals("json")) File.WriteAllText(path, jsonString);
                else this.SaveInXml<HistoryLog>(_myListHistoryLog, path);

            }

            else
            {
                                                                
                if (_configuration.LogFormat.Equals("json"))
                {

                    if (File.Exists(String.Format(@"{0}\Historylog-{1}.xml", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy"))))
                        this.GetListFromXml(String.Format(@"{0}\Historylog-{1}.xml", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy")));
                    _myListHistoryLog.Add(myHistoryLog);
                    string jsonString = JsonConvert.SerializeObject(_myListHistoryLog, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(path, jsonString);
                }

                else
                {
                    if (File.Exists(String.Format(@"{0}\Historylog-{1}.json", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy")))) this.GetListFromJson(String.Format(@"{0}\Historylog-{1}.json", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy")));
                    _myListHistoryLog.Add(myHistoryLog);
                    
                    this.SaveInXml<HistoryLog>(_myListHistoryLog, path);
                }
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
                File.Create(path).Close();
                return null;
            }
        }

        /// <summary>
        /// Get the list of HistoryLog from the old (json) log format file
        /// </summary>
        /// <param name="jsonPath"></param>
        public void GetListFromJson(string jsonPath)
        {
            Console.WriteLine(jsonPath);
            string json = File.ReadAllText(jsonPath);
            _myListHistoryLog = JsonConvert.DeserializeObject<List<HistoryLog>>(json);
            
            this.DeleteFile(jsonPath);
        }

        /// <summary>
        /// Get the list of HistoryLog from the old (xml) log format file
        /// </summary>
        /// <param name="path"></param>
        public void GetListFromXml(string path)
        {
            Console.WriteLine(path);
            string xml = File.ReadAllText(path);
            var serializer = new XmlSerializer(typeof(List<HistoryLog>));
            using TextReader reader = new StringReader(xml);
            _myListHistoryLog = (List<HistoryLog>)serializer.Deserialize(reader);
            Console.WriteLine(_myListHistoryLog);
            this.DeleteFile(path);
        }

        /// <summary>
        /// Delete a file and wait if the file is open
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFile(string path)
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
        /// Save a list of HistoryLog in Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logList"></param>
        /// <param name="path"></param>
        public void SaveInXml<T>(List<T> logList, string path)
        {
            var xmlserializer = new XmlSerializer(typeof(List<T>));
            using var writer = new StreamWriter(path);
            xmlserializer.Serialize(writer, logList);
            this.DeleteFile(String.Format(@"{0}\Historylog-{1}.json", _DEFAULT_LOG_FILE_PATH, DateTime.Now.ToString("d-MM-yyyy")));
        }

        /// <summary>Method which save a list of jobBackup into a json file</summary>
        /// <param name="jobBackupList"></param>
        public void SaveJobBackup(List<JobBackup> jobBackupList)
        {
            Directory.CreateDirectory(_DEFAULT_JOB_BACKUP_FILE_PATH);
            string jsonStringJobBackup = JsonConvert.SerializeObject(jobBackupList, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(String.Format(@"{0}\SavedJobBackup.json", _DEFAULT_JOB_BACKUP_FILE_PATH), jsonStringJobBackup);
        }

        /// <summary>Method which save a list of progressLog into a json file or xml format depending of user's choice</summary>
        /// <param name="progressLogList"></param>
        public void SaveProgressLog(ProgressLog myProgressLog, int index)
        {
            Directory.CreateDirectory(_DEFAULT_LOG_FILE_PATH);
            String path = String.Format(@"{0}\Progresslog.json", _DEFAULT_LOG_FILE_PATH);

            if (!File.Exists(path))
            {
                for (int i = 0; i < 5; i++)
                {
                    ProgressLog progressLog = new ProgressLog();
                    _myProgressLogList.Add(progressLog);
                }

                string jsonString = JsonConvert.SerializeObject(_myProgressLogList, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path, jsonString);
            }

            else
            {
                string file = File.ReadAllText(path);

                var myJsonList = JsonConvert.DeserializeObject<List<ProgressLog>>(file);
                myJsonList[index] = myProgressLog;
                _myProgressLogList = myJsonList;
                string jsonString = JsonConvert.SerializeObject(_myProgressLogList, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path, jsonString);

            }
        }
    }
}