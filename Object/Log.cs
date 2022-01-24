using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasySave
{
    /*
     <summary>
    Public Log class wich models a log
    </summary>
    */
    public class Log
    {

        protected static string _time;
        protected static string _sourceFile;
        protected static string _targetFile;
        protected static LogModel _myLogModel;
        protected static View _myView;

        public Log()
        {

        }

        /*Log class builder*/
        public Log(string sourceFile, string targetFile)
        {
            _sourceFile = sourceFile;
            _targetFile = targetFile;
            _time = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            _myView = new View();
            _myLogModel = new LogModel();
        }

        public bool SaveLog()
        {
            return true;
        }

        public string GetTime
        {
            get => _time;
        }

        public string GetSourcePath
        {
            get => _sourceFile;
        }

        public string GetTargetFile
        {
            get => _targetFile;
        }
    }
}
