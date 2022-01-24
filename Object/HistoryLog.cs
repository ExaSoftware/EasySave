using System;
using System.Collections.Generic;

namespace EasySave
{
    public class HistoryLog : Log
    {
        private int _fileSize;
        private string _transferTime;
        public HistoryLog(int fileSize, string transferTime) : base(_sourceFile, _targetFile)
        {
            _fileSize = fileSize;
            _transferTime = transferTime;
        }

        public int GetFileSize
        {
            get => this._fileSize;
        }

        public string GetTransferTime
        {
            get => this._transferTime;
        }

        new public bool SaveLog()
        {
            return _myLogModel.SaveHistoryLog(this);
        }
    }
}
