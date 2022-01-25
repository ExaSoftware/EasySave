﻿using Newtonsoft.Json;
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
        private int _fileSize;
        /// <summary>Transfer time of the file which have been saved</summary>
        private string _transferTime;

        /// <summary>History log builder</summary>
        public HistoryLog(string savedName, string sourceFile, string targetFile,int fileSize, string transferTime) : base(_label,_sourceFile, _targetFile)
        {
            _fileSize = fileSize;
            _transferTime = transferTime;
        }

        /// <summary>
        /// Getter which returns the file size
        /// </summary>
        /// <returns>The file size of the file which have been saved</returns>
        [JsonProperty(Order = 4)]
        public int FileSize
        {
            get => this._fileSize;
        }

        /// <summary>
        /// Getter which returns the file transfered time
        /// </summary>
        /// <returns>The file transfered time of the file which have been saved</returns>
        [JsonProperty(Order = 5)]
        public string TransferTime
        {
            get => this._transferTime;
        }

        /// <summary>
        /// Method which call SaveHistoryLog() from LogModel for created a history log file in C:\EasySave\Logs repository
        /// </summary>
        /// <returns>True if the history log file have been created False in the opposite case</returns>
        new public void SaveLog()
        {
            _myLogModel.SaveHistoryLog(this);
        }
    }
}
