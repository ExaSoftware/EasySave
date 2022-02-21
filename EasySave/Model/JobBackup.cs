using EasySave.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Windows;

namespace EasySave
{
    /// <summary>
    ///  JobBackup is a class. It allows you to save files from a directory to an other with differents methods.
    ///  Few constructors are available.
    /// </summary>
    public class JobBackup : IDisposable, INotifyPropertyChanged
    {
        // Attributes
        private string _label;
        private string _sourceDirectory;
        private string _destinationDirectory;
        private Boolean _isDifferential;
        private int _id;
        private string[] _encryptionExtensionList;
        private string[] _priorityExtensionList;
        private bool _disposedValue;
        private ProgressLog _state;
        private bool _isRunning;
        private int _priority;
        private ResourceManager _rm;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Properties
        public string SourceDirectory { get => _sourceDirectory; set => _sourceDirectory = value; }
        public string DestinationDirectory { get => _destinationDirectory; set => _destinationDirectory = value; }
        public bool IsDifferential { get => _isDifferential; set => _isDifferential = value; }
        public string Label { get => _label; set => _label = value; }
        public int Id { get => _id; set => _id = value; }
        public ProgressLog State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        public bool IsRunning { get => _isRunning; set => _isRunning = value; }

        ///  <summary> 
        ///  Default constructor to use in serialization.
        ///  </summary>
        public JobBackup() { }

        /// <summary>
        /// Create a JobBackup with default parameters.
        /// </summary>
        /// <param name="id">This parameter is the index of this JobBackup in the JobBackup list.</param>
        public JobBackup(int id)
        {
            _label = string.Empty;
            _sourceDirectory = string.Empty;
            _destinationDirectory = string.Empty;
            _isDifferential = false;
            _priority = 0;
            _id = id;
        }

        ///  <summary>
        ///  Constructors of JobBackup.
        ///  </summary>
        ///  <param name="Label"> the name of the save.</param>
        ///  <param name="SourceDirectory"> the source directory.</param>
        ///  <param name="DestinationDirectory"> the destination directory.</param>
        ///  <param name="IsDifferential"> Define if the save is differential or not.</param>
        public JobBackup(string label, string sourceDirectory, string destinationDirectory, bool isDifferential)
        {
            _label = label;
            _sourceDirectory = sourceDirectory;
            _destinationDirectory = destinationDirectory;
            _isDifferential = isDifferential;
        }

        ///  <summary>
        ///  Change parameters to the default one.
        ///  </summary>
        ///  <remarks>The Id still unchanged.</remarks>
        public void Fill(string label, string sourceDirectory, string destinationDirectory, bool isDifferential)
        {
            _label = label;
            _sourceDirectory = sourceDirectory;
            _destinationDirectory = destinationDirectory;
            _isDifferential = isDifferential;
        }

        ///  <summary>
        ///  Execute the save according to attributes.
        ///  </summary>
        ///  <remarks>Use it whether differential or not.</remarks>
        public void Execute()
        {
            bool error = false;

            if (!Directory.Exists(_destinationDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_destinationDirectory);
                }
                catch (Exception)
                {
                    error = true;
                }
            }

            if (!error)
            {
                _encryptionExtensionList = App.Configuration.Extensions;

                if (_isDifferential)
                {
                    DoDifferentialSave();
                }
                else
                {
                    SaveAllFiles();
                }
            }
        }

        ///  <summary>
        ///  Save all files from _sourceDirectory to _destDirectory.
        ///  </summary>
        ///  <remarks>This method delete all the destination directory before saving files</remarks>
        private void SaveAllFiles()
        {
            _isRunning = true;
            _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
            StringBuilder logSb = new StringBuilder();
            logSb.AppendLine(String.Format("{0} {1}", _rm.GetString("executionOf"), this.Label));

            int encryptionTime = 0;
            try
            {
                if (Monitor.TryEnter(_destinationDirectory, Timeout.Infinite))
                {
                    //Delete all files
                    Directory.Delete(_destinationDirectory, true);
                    //Restart from zero. 
                    Directory.CreateDirectory(_destinationDirectory);

                    //Creation of all sub directories
                    foreach (string path in Directory.GetDirectories(_sourceDirectory, "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(path.Replace(_sourceDirectory, _destinationDirectory));
                    }

                }
            }
            finally
            {
                Monitor.Exit(_destinationDirectory);
            }


            string[] files = Directory.GetFiles(_sourceDirectory, "*", SearchOption.AllDirectories);
            int fileTransfered = 0;                 //Incease each file transfered
            int fileToTranfer = files.Length;       //Ammount of file to transfer
            long sizeTotal = TotalFileSize(files);
            long sizeRemaining = sizeTotal;

            // Setup objects
            Stopwatch historyStopwatch = new Stopwatch();
            ProgressLog progressLog = new ProgressLog(_label, "", "", "ACTIVE", fileToTranfer, sizeTotal, fileToTranfer - fileTransfered, sizeTotal);
            HistoryLog historyLog = new HistoryLog(_label, "", "", 0, 0, 0);
            State = progressLog;
            State.Log = logSb.ToString();

            // Copy the files and overwrite destination files if they already exist.
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                long fileInfoLength = fileInfo.Length;
                string destFile = file.Replace(_sourceDirectory, _destinationDirectory);

                try
                {
                    historyStopwatch.Reset();

                    if (!(_encryptionExtensionList is null) && new List<string>(_encryptionExtensionList).Contains(fileInfo.Extension) && _encryptionExtensionList[0] != "")
                    {
                        encryptionTime = CypherFile(file, destFile);
                        string b = fileInfo.Extension;
                    }
                    else
                    {
                        historyStopwatch.Start();
                        File.Copy(file, destFile, true);
                        historyStopwatch.Stop();
                    }
                    fileTransfered++;
                    sizeRemaining -= fileInfoLength;

                    //Write logs
                    progressLog.Fill(file, destFile, (fileToTranfer - fileTransfered), (int)(100 - ((double)sizeRemaining / sizeTotal * 100)), _id, sizeRemaining);
                    historyLog.Fill(file, destFile, fileInfoLength, historyStopwatch.Elapsed.TotalMilliseconds, "", encryptionTime);
                    State = progressLog;
                }
                catch (Exception e)
                {
                    string fileName = Path.GetFileName(file);
                    destFile = Path.Combine(_destinationDirectory, fileName);
                    historyLog.Error = e.StackTrace;
                    historyLog.Fill(file, destFile, 0, -1, e.GetType().Name, -1);

                    //Show errors on file to the view
                    logSb.AppendLine(String.Format("{0} ==> {1}", _rm.GetString("errorFile"), file));
                    State.Log = logSb.ToString();
                    historyLog.Dispose();
                    progressLog.Dispose();
                }
                finally
                {
                    //Free memory
                    fileInfo = null;
                    destFile = string.Empty;

                    //dispose history and progress log
                    historyLog.Dispose();
                    progressLog.Dispose();
                }
            }

            logSb.AppendLine(_rm.GetString("executionFinished"));
            State.Log = logSb.ToString();
            logSb = null;
            _isRunning = false;
            State.State = "END";

        }

        /// <summary> 
        /// Save all differents files between _sourceDirectory and _destDirectory to _destDirectory.
        /// </summary>
        /// <remarks>This method ignores deleted files.</remarks>
        private void DoDifferentialSave()
        {
            _isRunning = true;
            _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());

            StringBuilder logSb = new StringBuilder();
            logSb.AppendLine(String.Format("{0} {1}", _rm.GetString("executionOf"), this.Label));

            int encryptionTime;

            String[] files = FindFilesForDifferentialSave();

            //Creation of all sub directories
            foreach (string path in Directory.GetDirectories(_sourceDirectory, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(path.Replace(_sourceDirectory, _destinationDirectory));
            }

            int fileTransfered = 0;                 //Ammount of file transfered
            int fileToTranfer = files.Length;       //Ammount of file to transfer
            long sizeTotal = TotalFileSize(files);  //Total file size in octet
            long sizeRemaining = sizeTotal;

            Stopwatch historyStopwatch = new Stopwatch();
            ProgressLog progressLog = new ProgressLog(_label, "", "", "ACTIVE", fileToTranfer, sizeTotal, fileToTranfer - fileTransfered, sizeRemaining);
            HistoryLog historyLog = new HistoryLog(_label, "", "", 0, 0, 0);

            //Affect progressLog to state attribute of JobBackup and update log string
            State = progressLog;
            State.Log = logSb.ToString();

            foreach (String file in files)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);

                    // Creation of the destFile
                    string destFile = file.Replace(_sourceDirectory, _destinationDirectory);

                    if (!(_encryptionExtensionList is null) && new List<String>(_encryptionExtensionList).Contains(fileInfo.Extension) && _encryptionExtensionList[0] != "")
                    {
                        encryptionTime = CypherFile(file, destFile);
                    }
                    else
                    {
                        encryptionTime = 0;
                        historyStopwatch.Reset();
                        historyStopwatch.Start();

                        File.Copy(file, destFile, true);

                        historyStopwatch.Stop();
                    }

                    fileTransfered++;
                    sizeRemaining -= fileInfo.Length;

                    progressLog.Fill(file, destFile, fileToTranfer - fileTransfered, (int)(100 - ((double)sizeRemaining / sizeTotal * 100)), _id, sizeRemaining);
                    historyLog.Fill(file, destFile, fileInfo.Length, historyStopwatch.Elapsed.TotalMilliseconds, "", encryptionTime);
                    State = progressLog;
                }
                catch (Exception e)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.Error = e.StackTrace;
                    historyLog.Fill(file, destFile, 0, -1, file, -1);

                    logSb.AppendLine(String.Format("{0} ==> {1}", _rm.GetString("errorFile"), file));
                    State.Log = logSb.ToString();

                    historyLog.Dispose();
                    progressLog.Dispose();
                }
            }

            //Delete excess directories
            foreach (string path in Directory.GetDirectories(_destinationDirectory, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(path.Replace(_destinationDirectory, _sourceDirectory)) && Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path, true);
                    }
                    catch { }
                }
            }

            //Delete excess files
            foreach (string path in Directory.GetFiles(_destinationDirectory, "*", SearchOption.AllDirectories))
            {
                if (!File.Exists(path.Replace(_destinationDirectory, _sourceDirectory)) && File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch { }
                }
            }

            //Show logs
            logSb.AppendLine(_rm.GetString("executionFinished"));
            State.Log = logSb.ToString();
            logSb = null;
            _isRunning = false;


            //Reset progressLog
            
            //progressLog.Reset(_id);
            historyLog.Dispose();
            progressLog.Dispose();
            State.State = "END";
        }

        /// <summary>
        /// Return the sum of the size of each file wich will be transfered.
        /// </summary>
        /// <returns>The total size un octet</returns>
        public long TotalFileSize()
        {
            return Directory.Exists(_sourceDirectory)
                ? _isDifferential
                    ? TotalFileSize(FindFilesForDifferentialSave())
                    : TotalFileSize(Directory.GetFiles(_sourceDirectory, "*", SearchOption.AllDirectories))
                : 0;
        }

        /// <summary>
        /// Return the sum of the size of each files.
        /// </summary>
        /// <param name="files">A list of files. Must be a Path format</param>
        /// <remarks>Invoke the Garbage Collector at the end.</remarks>
        /// <returns>The total size un octet</returns>
        private long TotalFileSize(String[] files)
        {
            long totalSize = 0;

            foreach (String file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                totalSize += fileInfo.Length;
            }

            GC.Collect();
            return totalSize;
        }

        /// <summary>
        /// Compare all files between the source and the dest directories. 
        /// Use the default hashCode to compare.
        /// </summary>
        /// <returns>A list of diferent files between source and dest directories.</returns>
        private string[] FindFilesForDifferentialSave()
        {
            List<String> filesToSave = new List<string>();
            String[] filesInDirectory = Directory.GetFiles(_sourceDirectory, "*", SearchOption.AllDirectories);

            foreach (String file in filesInDirectory)
            {
                // Creation of the destFile
                string destFile = file.Replace(_sourceDirectory, _destinationDirectory);

                if (File.Exists(destFile))
                {
                    //Get the hashCode of source file
                    int inComingFileHash = File.ReadAllBytes(file).GetHashCode();
                    //Get the hashCode of the destFle
                    int destinationFileHash = File.ReadAllBytes(destFile).GetHashCode();

                    if (inComingFileHash != destinationFileHash)
                    {
                        filesToSave.Add(file);
                    }
                }
                else
                {
                    filesToSave.Add(file);
                }
            }

            return filesToSave.ToArray();
        }

        /// <summary>
        /// This method use CryptoSoft to encode or decode and copy a file.
        /// The key is auto generated and store at C:\EasySave\CryptoSoft\config
        /// </summary>
        /// <remarks>You can use it in both ways.</remarks>
        /// <param name="sourceFile">The file to encode. Must be a Path format.</param>
        /// <param name="destFile">The destination file. Must be a Path format.</param>
        /// <returns>The encryption time.</returns>
        private int CypherFile(String sourceFile, String destFile)
        {
            Process process = new Process();
            int time;

            process.StartInfo.FileName = @"C:\EasySave\CryptoSoft\bin\Debug\netcoreapp3.1\CryptoSoft.exe";
            process.StartInfo.Arguments = String.Format("\"{0}\" \"{1}\"", sourceFile, destFile);
            Trace.WriteLine(process.StartInfo.Arguments);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            if (process.HasExited)
            {
                time = Convert.ToInt32(process.StandardOutput.ReadToEnd());
            }
            else
            {
                process.WaitForExit();
                time = Convert.ToInt32(process.StandardOutput.ReadToEnd());
            }

            process.Close();
            return time;
        }

        /// <summary>
        /// Inherited from iDisposable.
        /// Used to kill this object.
        /// </summary>
        /// <param name="disposing">Avoid double execution with GC.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés)
                    _label = string.Empty;
                    _sourceDirectory = string.Empty;
                    _destinationDirectory = string.Empty;
                    _encryptionExtensionList = null;
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                _disposedValue = true;
            }
        }

        // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        /*~JobBackup()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: false);
        }*/

        /// <summary>
        /// Inherited from iDisposable. Use this method to kill this object.
        /// This method will free as memory as possible and tag this object.
        /// </summary>
        /// <remarks>The GarbageCollector is not called.</remarks>
        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}