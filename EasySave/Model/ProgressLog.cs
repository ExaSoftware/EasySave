using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EasySave.Object
{
    public class ProgressLog : Log, INotifyPropertyChanged
    {
        /// <summary>State of the saving job</summary>
        private string _state;
        /// <summary>The amout of files which have been copy</summary>
        private int _totalFilesToCopy;
        /// <summary>The total amount size of the job which is being saved</summary>
        private long _totalFilesSize;
        /// <summary>The size remaining to copy</summary>
        private long _sizeRemaining;
        /// <summary>The number of files which is remaining to copy </summary>
        private int _totalFilesRemaining;
        /// <summary>The current progression of the job saving</summary>
        private int _progression = 0;

        private string _log = String.Empty;

        /// <summary>
        /// Progress log builder
        /// </summary>
        /// <param name="savedName"></param>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <param name="state"></param>
        /// <param name="totalFilesToCopy"></param>
        /// <param name="totalFilesSize"></param>
        /// <param name="totalFilesRemaining"></param>
        public ProgressLog(string savedName, string sourceFile, string targetFile, string state, int totalFilesToCopy, long totalFilesSize, int totalFilesRemaining, long sizeRemaining) : base(savedName, sourceFile, targetFile)
        {
            _state = state;
            _totalFilesToCopy = totalFilesToCopy;
            _totalFilesSize = totalFilesSize;
            _totalFilesRemaining = totalFilesRemaining;
            _sizeRemaining = sizeRemaining;
        }

        [JsonProperty(Order = 4)]
        public string State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }  
        }
        [JsonProperty(Order = 5)]
        public int TotalFilesToCopy
        {
            get => _totalFilesToCopy;
            set
            {
                _totalFilesToCopy = value;
                OnPropertyChanged("TotalFilesToCopy");
            }
        }
        [JsonProperty(Order = 6)]
        public long TotalFilesSize 
        {
            get => _totalFilesSize;
            set
            {
                _totalFilesSize = value;
                OnPropertyChanged("TotalFilesSize");
            }
        }
        [JsonProperty(Order = 7)]
        public int TotalFilesRemaining
        {
            get => _totalFilesRemaining;
            set
            {
                _totalFilesRemaining = value;
                OnPropertyChanged("TotalFilesRemaining");
            }
        }

        [JsonProperty(Order = 8)]
        public long SizeRemaining 
        {
            get => _sizeRemaining;
            set
            {
                _sizeRemaining = value;
                OnPropertyChanged("SizeRemaining");
            }  
        }

        [JsonProperty(Order = 9)]
        public int Progression
        {
            get => _progression;
            set
            {
                _progression = value;
                OnPropertyChanged("Progression");
            }
        }

        public double SizeRemainingFormatted
        {
            get
            {
                return Math.Round((double)_sizeRemaining / 1048576, 2);
            }

        }

        public string Log
        { 
            get => _log;
            set
            {
                _log = value;
                OnPropertyChanged("Log");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void Fill(string file, string destFile, int totalFilesRemaining, int progression, int id, long sizeRemaining)
        {
            _state = "ACTIVE";
            this._sourceFile = file;
            this._targetFile = destFile;
            this._totalFilesRemaining = totalFilesRemaining;
            this._progression = progression;
            this._sizeRemaining = sizeRemaining;
            this.SaveLog();
        }

        public void Reset(int id)
        {
            this._sourceFile = "";
            this._targetFile = "";
            this._state = "END";
            this._totalFilesRemaining = 0;
            this._totalFilesToCopy = 0;
            this._totalFilesSize = 0;
            this._sizeRemaining = 0;
            this._progression = 100;
            this.SaveLog();
        }

        /// <summary>
        /// Method which call SaveProgressLog() from LogModel for created a progress log file in C:\EasySave\Logs repository
        /// </summary>
        public override void SaveLog()
        {
            JsonReadWriteModel.SaveProgressLog(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés)
                    this._sourceFile = String.Empty;
                    this._targetFile = String.Empty;
                    this._state = String.Empty;
                    this._totalFilesRemaining = 0;
                    this._totalFilesToCopy = 0;
                    this._totalFilesSize = 0;
                    this._progression = 0;
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null

                disposedValue = true;
            }
        }
    }
}