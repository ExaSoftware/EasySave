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

        /// <summary>
        /// Method which call SaveProgressLog() from LogModel for created a progress log file in C:\EasySave\Logs repository
        /// </summary>
        public override void SaveLog(List<ProgressLog> progressLogList)
        {
            _myLogModel.SaveProgressLog(progressLogList);
        }
    }
}
