using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace EasySave
{
    [JsonConverter(typeof(HistoryLog))]
    [XmlRoot(ElementName = "HistoryLog")]
    /// <summary>
    /// This class models an history log of what have been saved for a specific job save
    /// </summary>
    public class HistoryLog : Log
    {
        [XmlElement(ElementName = "fileSize")]
        /// <summary>The file size of the file which have been saved</summary>
        private ulong _fileSize;
        [XmlElement(ElementName = "transferTime")]
        /// <summary>Transfer time of the file which have been saved</summary>
        private double _transferTime;
        [XmlElement(ElementName = "error")]
        ///<summary> Error from the stackTrace </summary>
        private string _error;
        [XmlElement(ElementName = "time")]
        /// <summary> Timestamp when the job have been saved </summary>
        private string _time;
        [XmlElement(ElementName = "encryptionTime")]
        ///<sumary>Necessary time to encrypt the file</summary>
        protected double _encryptionTime;
        [XmlElement(ElementName = "errorTitle")]
        /// <summary>Code error needed in the View to notify the user</summary>
        protected string _errorTitle;

        private JsonReadWriteModel _jsonReadWriteModel = new JsonReadWriteModel();

        public HistoryLog() { }

        /// <summary>History log builder</summary>
        public HistoryLog(string label, string sourceFile, string targetFile, ulong fileSize, double transferTime, double encryptionTime) : base(label, sourceFile, targetFile)
        {
            _fileSize = fileSize;
            _transferTime = transferTime;
            _time = DateTime.Now.ToString("d/MM/yyyy HH:mm:ss");
            _encryptionTime = encryptionTime;
            _errorTitle = "";
            _error = "";
        }

        /// <summary>
        /// Getter which returns the file size
        /// </summary>
        /// <returns>The file size of the file which have been saved</returns>
        [JsonProperty(Order = 4)]
        public ulong FileSize
        {
            get => this._fileSize;
            set => _fileSize = value;
        }

        /// <summary>
        /// Getter which returns the file transfered time
        /// </summary>
        /// <returns>The file transfered time of the file which have been saved</returns>
        [JsonProperty(Order = 5)]
        public double TransferTime
        {
            get => this._transferTime;
            set => _transferTime = value;
        }

        /// <summary>
        /// Getter which returns the timestamp at the creation of the history log
        /// </summary>
        /// <returns>When the history log have been created at the format (dddd, dd MMMM yyyy HH:mm:ss)</returns>*
        [JsonProperty(Order = 6)]
        public string Time
        {
            get => _time;
            set => _time = value;
        }

        [JsonProperty(Order = 7)]
        public double EncryptionTime
        {
            get => _encryptionTime;
            set => _encryptionTime = value;
        }

        [JsonProperty(Order = 8)]
        public string Error
        {
            get => _error;
            set => _error = value;
        }

        [JsonProperty(Order = 9)]
        public string ErrorTitle
        {
            get => _errorTitle;
            set => _errorTitle = value;
        }
        /// <summary>
        /// Method which call SaveHistoryLog() from LogModel for created a history log file in C:\EasySave\Logs repository
        /// </summary>
        /// <returns>True if the history log file have been created False in the opposite case</returns>
        public override void SaveLog(int id)
        {
            _jsonReadWriteModel.SaveHistoryLog(this,id);
        }

        public void Fill(string sourceFile, string targetFile, long fileSize, double transfertTime, string error, int encryptionTime,int id)
        {
            this.SourceFile = sourceFile;
            this.TargetFile = targetFile;
            this.FileSize = (ulong)fileSize;
            this.TransferTime = transfertTime;
            this.ErrorTitle = error;
            this.EncryptionTime = encryptionTime;
            this.SaveLog(id);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés)
                    this.SourceFile = String.Empty;
                    this.TargetFile = String.Empty;
                    this.FileSize = (ulong)0;
                    this.TransferTime = 0;
                    this.Error = String.Empty;
                    this.EncryptionTime = 0;
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null

                disposedValue = true;
            }
        }
    }
}
