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
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
        private ulong _sizeLimit;
        private int _priority;

        private bool _disposedValue;

        private ProgressLog _state;
        private ResourceManager _rm;
        private bool _isRunning;

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
        public int Priority 
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged("Priority");
            }  
        }

        ///  <summary> 
        ///  Default constructor to use in serialization.
        ///  </summary>
        public JobBackup()
        {
            _isRunning = false;
        }

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
            _id = id;
            _priority = 0;
            _isRunning = false;
        }

        ///  <summary>
        ///  Constructors of JobBackup.
        ///  </summary>
        ///  <param name="Label"> the name of the save.</param>
        ///  <param name="SourceDirectory"> the source directory.</param>
        ///  <param name="DestinationDirectory"> the destination directory.</param>
        ///  <param name="IsDifferential"> Define if the save is differential or not.</param>
        public JobBackup(string label, string sourceDirectory, string destinationDirectory, bool isDifferential, int priority)
        {
            _label = label;
            _sourceDirectory = sourceDirectory;
            _destinationDirectory = destinationDirectory;
            _isDifferential = isDifferential;
            _priority = priority;
            _isRunning = false;
        }

        ///  <summary>
        ///  Change parameters to the default one.
        ///  </summary>
        ///  <remarks>The Id still unchanged.</remarks>
        public void Fill(string label, string sourceDirectory, string destinationDirectory, bool isDifferential, int priority)
        {
            _label = label;
            _sourceDirectory = sourceDirectory;
            _destinationDirectory = destinationDirectory;
            _isDifferential = isDifferential;
            _priority = priority;
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
                    _ = Directory.CreateDirectory(_destinationDirectory);
                }
                catch (Exception)
                {
                    error = true;
                }
            }

            if (!error)
            {
                _isRunning = true;
                _encryptionExtensionList = App.Configuration.Extensions;
                _sizeLimit = App.Configuration.SizeLimit;

                if (_isDifferential)
                {
                    SaveFiles(FindFilesForDifferentialSave());
                    DeleteExcessFile();
                }
                else
                {
                    DeleteFiles();
                    SaveFiles(Directory.GetFiles(_sourceDirectory, "*", SearchOption.AllDirectories));
                }

                _isRunning = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="saveBigFiles"></param>
        private void SaveFiles(string[] files)
        {
            _rm = new ResourceManager("EasySave.Resources.Strings", Assembly.GetExecutingAssembly());
            ObservableCollection<string> temp = new ObservableCollection<string>();

            int encryptionTime = 0;
            int fileTransfered = 0;                 //Incease each file transfered
            int fileToTranfer = files.Length;       //Ammount of file to transfer
            long sizeTotal = TotalFileSize(files);
            long sizeRemaining = sizeTotal;
            int errors = 0;

            // Setup objects
            Stopwatch historyStopwatch = new Stopwatch();
            ProgressLog progressLog = new ProgressLog(_label, "", "", "ACTIVE", fileToTranfer, sizeTotal, fileToTranfer - fileTransfered, sizeTotal);
            HistoryLog historyLog = new HistoryLog(_label, "", "", 0, 0, 0);
            State = progressLog;

            //Show message which say that job backup is executing
            App.Current.Dispatcher.Invoke(delegate
            {
                State.Log.Add(string.Format("{0} '{1}' {2} {3}. ", _rm.GetString("executionOf"), Label, _rm.GetString("at"), DateTime.Now.ToString("T")));
            });
            //Add the string to conserv it for the final display
            temp.Add(string.Format("{0} '{1}' {2} {3} ", _rm.GetString("executionOf"), Label, _rm.GetString("at"), DateTime.Now.ToString("T")));

            string realDest = Path.Combine(_destinationDirectory, Path.GetFileName(_sourceDirectory));

            if (Monitor.TryEnter(_destinationDirectory, Timeout.Infinite))
            {
                try
                {
                    //Delete all files if it's not differential
                    if (Directory.Exists(realDest) && !_isDifferential)
                    {
                        Directory.Delete(realDest, true);
                    }

                    //Restart from zero. 
                    _ = Directory.CreateDirectory(realDest);

                    //Creation of all sub directories
                    foreach (string path in Directory.GetDirectories(_sourceDirectory, "*", SearchOption.AllDirectories))
                    {
                        _ = Directory.CreateDirectory(path.Replace(_sourceDirectory, realDest));
                    }
                }
                finally
                {
                    Monitor.Exit(_destinationDirectory);
                }
            }


            // Copy the files and overwrite destination files if they already exist.
            foreach (string file in files)
            {
                if (!App.ThreadPause)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    long fileInfoLength = fileInfo.Length;
                    string destFile = file.Replace(_sourceDirectory, realDest);


                    try
                    {
                        historyStopwatch.Reset();

                        if ((ulong)fileInfo.Length > _sizeLimit && _sizeLimit > 0)
                        {
                            if (Monitor.TryEnter(App.IsMovingBigFile, Timeout.Infinite))
                            {
                                while (true)
                                {
                                    if (!App.IsMovingBigFile)
                                    {
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
                                        sizeRemaining -= fileInfo.Length; ;

                                        //Write logs
                                        Task bigTask = Task.Run(() =>
                                        {
                                            progressLog.Fill(file, destFile, fileToTranfer - fileTransfered, _id, (int)(100 - ((double)sizeRemaining / sizeTotal * 100)), sizeRemaining, fileToTranfer, sizeTotal);
                                            historyLog.Fill(file, destFile, fileInfoLength, historyStopwatch.Elapsed.TotalMilliseconds, "", encryptionTime, _id);
                                        });

                                        bigTask.Wait();
                                        bigTask.Dispose();
                                        State = progressLog;
                                        break;
                                    }
                                    else
                                    {
                                        Thread.Sleep(10);
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {

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
                            sizeRemaining -= fileInfo.Length; ;

                            //Write logs
                            Task task = Task.Run(() =>
                                {
                                    progressLog.Fill(file, destFile, fileToTranfer - fileTransfered, _id, (int)(100 - ((double)sizeRemaining / sizeTotal * 100)), sizeRemaining, fileToTranfer, sizeTotal);
                                    historyLog.Fill(file, destFile, fileInfoLength, historyStopwatch.Elapsed.TotalMilliseconds, "", encryptionTime, _id);
                                });

                            task.Wait();
                            task.Dispose();
                            State = progressLog;

                        }
                    }


                    catch (Exception e)
                    {
                        destFile = Path.Combine(_destinationDirectory, Path.GetFileName(file));
                        fileTransfered++;

                        historyLog.Error = e.StackTrace;
                        historyLog.Fill(file, destFile, 0, -1, e.GetType().Name, -1, _id);

                        //Add error to the temp list
                        errors++;
                        temp.Add(String.Format("{0} ==> {1}", _rm.GetString("errorFile"), file));
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


                // If pause
                else
                {
                    while (App.ThreadPause) { Thread.Sleep(1000); }
                }

            }
            string result = String.Empty;
            if (errors == 0)
            {
                result = String.Format("{0} {1} {2}.     {3}", _rm.GetString("executionFinished"), _rm.GetString("at"), DateTime.Now.ToString("T"), _rm.GetString("noError"));
            }
            else
            {
                result = String.Format("{0} {1} {2}.     {3} {4}:", _rm.GetString("executionFinished"), _rm.GetString("at"), DateTime.Now.ToString("T"), errors, _rm.GetString("errors"));
            }
            Application.Current.Dispatcher.Invoke(delegate
            {
                //Insert the execution finished string at the begin of the list to show
                temp.Insert(1, result);
                State.Log = temp;
                temp = null;
            });

            _isRunning = false;
            State.State = "END";

        }


        private void DeleteExcessFile()
        {
            string realDest = Path.Combine(_destinationDirectory, Path.GetFileName(_sourceDirectory));

            //Delete excess directories
            foreach (string path in Directory.GetDirectories(_destinationDirectory, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(path.Replace(realDest, _sourceDirectory)) && Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path, true);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            //Delete excess files
            foreach (string path in Directory.GetFiles(_destinationDirectory, "*", SearchOption.AllDirectories))
            {
                if (!File.Exists(path.Replace(realDest, _sourceDirectory)) && File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        private void DeleteFiles()
        {
            string realDest = Path.Combine(_destinationDirectory, Path.GetFileName(_sourceDirectory));

            try
            {
                if (Monitor.TryEnter(_destinationDirectory, 60000))
                {
                    //Delete all files
                    if (Directory.Exists(realDest))
                    {
                        Directory.Delete(realDest, true);
                    }
                }
            }
            catch
            {

            }
            finally
            {
                Monitor.Exit(_destinationDirectory);

                //Restart from zero. 
                _ = Directory.CreateDirectory(realDest);

                //Creation of all sub directories
                foreach (string path in Directory.GetDirectories(_sourceDirectory, "*", SearchOption.AllDirectories))
                {
                    _ = Directory.CreateDirectory(path.Replace(_sourceDirectory, realDest));
                }
            }
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
            List<string> filesToSave = new List<string>();
            string[] filesInDirectory = Directory.GetFiles(_sourceDirectory, "*", SearchOption.AllDirectories);

            foreach (string file in filesInDirectory)
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
        private int CypherFile(string sourceFile, string destFile)
        {
            Process process = new Process();
            int time;

            process.StartInfo.FileName = String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"CryptoSoft\CryptoSoft.exe");
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