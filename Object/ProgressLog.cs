using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Object
{
    public class ProgressLog : Log
    {
        private string _state;
        private int _totalFilesToCopy;
        private long _totalFilesSize;
        private int _totalFilesRemaining;
        private int _progression = 0;

        ///  <summary>Create a ProgressLog object.</summary>
        ///  <remarks>This method set attributes with default parameters.</remarks>
        public ProgressLog()
        {
            this._label = "";
            this._sourceFile = "";
            this._targetFile = "";
            this._state = "END";
            this._totalFilesToCopy = 0;
            this._totalFilesSize = 0;
            this._totalFilesRemaining = 0;
            this._progression = 0;
        }

        public ProgressLog(string savedName, string sourceFile, string targetFile, string state, int totalFilesToCopy, long totalFilesSize, int totalFilesRemaining) : base(savedName, sourceFile, targetFile)
        {
            _state = state;
            _totalFilesToCopy = totalFilesToCopy;
            _totalFilesSize = totalFilesSize;
            _totalFilesRemaining = totalFilesRemaining;
        }

        [JsonProperty(Order = 4)]
        public string State { get => _state; set => _state = value; }
        [JsonProperty(Order = 5)]
        public int TotalFilesToCopy { get => _totalFilesToCopy; set => _totalFilesToCopy = value; }
        [JsonProperty(Order = 6)]
        public long TotalFilesSize { get => _totalFilesSize; set => _totalFilesSize = value; }
        [JsonProperty(Order = 7)]
        public int TotalFilesRemaining { get => _totalFilesRemaining; set => _totalFilesRemaining = value; }
        [JsonProperty(Order = 8)]
        public int Progression { get => _progression; set => _progression = value; }

        /// <summary>
        /// Method which call SaveProgressLog() from LogModel for created a progress log file in C:\EasySave\Logs repository
        /// </summary>
        public override void SaveLog(int index)
        {
            _myLogModel.SaveProgressLog(this, index);
        }
    }
}
