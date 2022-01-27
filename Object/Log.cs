using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave
{
    ///<summary>Public Log class which models a log file</summary>

    public abstract class Log
    {
        /// <summary> Job save label </summary>
        public static string _label;
        /// <summary> Timestamp when the job have been saved </summary>
        protected static string _time;
        /// <summary> Source file path of the file which have been saved </summary>
        protected static string _sourceFile;
        /// <summary> Target file path of the file which have been saved </summary>
        protected static string _targetFile;
        protected static LogModel _myLogModel;

        ///<summary>Log class builder</summary>
        public Log()
        {

        }

        ///<summary>Log class builder</summary>
        ///aram name=savedName>The name of the save Job</param>
        ///<param name=sourceFile>The source file path</param>
        ///<param name=targetFile>The target file path</param>
        public Log(string savedName,string sourceFile, string targetFile)
        {
            _label = savedName;
            _sourceFile = sourceFile;
            _targetFile = targetFile;
            _time = DateTime.Now.ToString("d/MM/yyyy HH:mm:ss");
            _myLogModel = new LogModel();
        }

        /// <summary>
        /// Method which doing nothing but is useful for polymorphism
        /// </summary>
        public virtual void SaveLog()
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
    }
}
