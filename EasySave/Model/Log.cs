using Newtonsoft.Json;
using System;
using System.Text.Json;

namespace EasySave
{
    ///<summary>Public Log class which models a log file</summary>
    public abstract class Log : IDisposable
    {
        /// <summary> Job save label </summary>
        protected string _label;
        /// <summary> Source file path of the file which have been saved </summary>
        protected string _sourceFile;
        /// <summary> Target file path of the file which have been saved </summary>
        protected string _targetFile;

        protected JsonReadWriteModel _myLogModel;
        protected bool disposedValue;

        ///<summary>Log class builder</summary>
        public Log()
        {

        }

        ///<summary>Log class builder</summary>
        ///param name=savedName>The name of the save Job</param>
        ///<param name=sourceFile>The source file path</param>
        ///<param name=targetFile>The target file path</param>
        public Log(string label, string sourceFile, string targetFile)
        {
            _label = label;
            _sourceFile = sourceFile;
            _targetFile = targetFile;
            _myLogModel = new JsonReadWriteModel();
        }

        /// <summary>
        /// Method which doing nothing but is useful for polymorphism
        /// </summary>
        public virtual void SaveLog()
        {

        }

        public virtual void SaveLog(int index)
        {

        }

        /// <summary>
        /// Getter which returns the history log name
        /// </summary>
        /// <returns>The history log name</returns>
        [JsonProperty(Order = 1)]
        public string Name
        {
            get => _label;
            set => _label = value;
        }

        /// <summary>
        /// Getter which returns the source file path of the file which have been saved
        /// </summary>
        /// <returns>The source file path of the file which have been saved</returns>
        [JsonProperty(Order = 2)]
        public string SourceFile
        {
            get => _sourceFile;
            set => _sourceFile = value;
        }
        /// <summary>
        /// Getter which returns the target file path of the file which have been saved
        /// </summary>
        /// <returns>The target file path of the file which have been saved</returns>
        [JsonProperty(Order = 3)]
        public string TargetFile
        {
            get => _targetFile;
            set => _targetFile = value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés)
                    _label = String.Empty;
                    _sourceFile = String.Empty;
                    _targetFile = String.Empty;
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                _myLogModel = null;
                disposedValue = true;
            }
        }

        // substituer le finaliseur uniquement si 'Dispose(bool disposing) a du code pour libérer les ressources non managées
        ~Log()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
