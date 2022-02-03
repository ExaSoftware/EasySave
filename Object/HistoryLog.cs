using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EasySave
{
    /// <summary>
    /// This class models an history log of what have been saved for a specific job save
    /// </summary>
    public class HistoryLog : Log
    {
        /// <summary>The file size of the file which have been saved</summary>
        private ulong _fileSize;
        /// <summary>Transfer time of the file which have been saved</summary>
        private double _transferTime;

        private string _error;
        /// <summary> Timestamp when the job have been saved </summary>
        protected string _time;

        ///<sumary>Necessary time to encrypt the file</summary>
        protected double _encryptionTime;

        /// <summary>Code error needed in the View to notify the user</summary>
        protected string _codeError;

        /// <summary>History log builder</summary>
        public HistoryLog(string label, string sourceFile, string targetFile, ulong fileSize, double transferTime, double encryptionTime) : base(label, sourceFile, targetFile)
        {
            _fileSize = fileSize;
            _transferTime = transferTime;
            _time = DateTime.Now.ToString("d/MM/yyyy HH:mm:ss");
            _encryptionTime = encryptionTime;
            _codeError = "";
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
        public double NeccessaryEncryptionTime
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
        public string CodeError
        {
            get => _codeError;
            set => _codeError = value;
        }
        /// <summary>
        /// Method which call SaveHistoryLog() from LogModel for created a history log file in C:\EasySave\Logs repository
        /// </summary>
        /// <returns>True if the history log file have been created False in the opposite case</returns>
        public override void SaveLog()
        {
            _myLogModel.SaveHistoryLog(this);
        }
    }
}
