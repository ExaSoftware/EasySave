using System;
using System.IO;
using System.Diagnostics;
using static EasySave.JobBackUpModel;
using EasySave.Object;
using System.Collections.Generic;

namespace EasySave
{
    /// <summary>
    ///  JobBackup is a Picasso class. It allows you to save files from a directory to an other with differents methods.
    ///  Few constructors are available.
    /// </summary>
    public class JobBackup : IDisposable
    {
        // Attributes
        private String _label;
        private String _sourceDirectory;
        private String _destinationDirectory;
        private Boolean _isDifferential;
        private int _id;
        private List<String> _extensionList = new List<string> { "pdf", "xlsx" };
        private bool _disposedValue;

        // Properties
        public string SourceDirectory { get => _sourceDirectory; set => _sourceDirectory = value; }
        public string DestinationDirectory { get => _destinationDirectory; set => _destinationDirectory = value; }
        public bool IsDifferential { get => _isDifferential; set => _isDifferential = value; }
        public string Label { get => _label; set => _label = value; }
        public int Id { get => _id; set => _id = value; }

        ///  <summary> Default constructor to use in serialization.</summary>
        public JobBackup() { }

        public JobBackup(int id)
        {
            _label = "";
            _sourceDirectory = "";
            _destinationDirectory = "";
            _isDifferential = false;
            this._id = id;
        }

        ///  <summary>Constructors of JobBackup.</summary>
        ///  <param name="Label"> the name of the save.</param>
        ///  <param name="SourceDirectory"> the source directory.</param>
        ///  <param name="DestinationDirectory"> the destination directory.</param>
        ///  <param name="IsDifferential"> Define if the save is differential or not.</param>
        public JobBackup(String label, String sourceDirectory, String destinationDirectory, Boolean isDifferential)
        {
            this._label = label;
            this._sourceDirectory = sourceDirectory;
            this._destinationDirectory = destinationDirectory;
            this._isDifferential = isDifferential;
        }

        ///  <summary>Change parameters to the default one.</summary>
        ///  <returns>The jobBackup edited</returns> 
        ///  <remarks>The Id still unchanged.</remarks>
        public JobBackup Reset()
        {
            _label = "";
            _sourceDirectory = "";
            _destinationDirectory = "";
            _isDifferential = false;
            return this;
        }

        ///  <summary>Execute the save according to attributes.</summary>
        ///  <remarks>Use it whether differential or not.</remarks>
        public bool Execute(List<string> buisnessSoftwareName)
        {
            bool error = false;
            int i = 0;
            while (buisnessSoftwareName.Count > i )
            {
                Process[] procName = Process.GetProcessesByName(buisnessSoftwareName[i]);
                if (procName.Length != 0)
                {
                    return true;
                }
                i -= 1;
            } 
               
            if (!Directory.Exists(_destinationDirectory))
            {
                try
                {
                    Directory.CreateDirectory(DestinationDirectory);
                }
                catch (ArgumentException)
                {
                    error = true;

                }
                catch (Exception)
                {
                    error = true;
                }
            }

            if (!error)
            {
                if (_isDifferential)
                {
                    error = DoDifferentialSave();
                }
                else
                {
                    error = SaveAllFiles();
                }
            }
            return error;
        }
                   

        ///  <summary>Save all files from _sourceDirectory to _destDirectory.</summary>
        ///  <remarks>This method ignores deleted files.</remarks>
        private bool SaveAllFiles()
        {
            bool error = false;

            //Creation of all sub directories
            foreach (string path in Directory.GetDirectories(_sourceDirectory, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(path.Replace(_sourceDirectory, _destinationDirectory));
            }

            string[] files = Directory.GetFiles(_sourceDirectory, "*", SearchOption.AllDirectories);
            int fileTransfered = 0;                 //Incease each file transfered
            int fileToTranfer = files.Length;       //Ammount of file to transfer
            long sizeTotal = TotalFileSize(files);

            // Setup objects
            Stopwatch historyStopwatch = new Stopwatch();
            ProgressLog progressLog = new ProgressLog(_label, "", "", "ACTIVE", fileToTranfer, sizeTotal, fileToTranfer - fileTransfered);
            HistoryLog historyLog = new HistoryLog(_label, "", "", 0, 0,0);

            // Copy the files and overwrite destination files if they already exist.
            foreach (string file in files)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);
                    string destFile = file.Replace(_sourceDirectory, _destinationDirectory);
                    int encryptionTime;

                    historyStopwatch.Reset();

                    if (_extensionList.Contains(fileInfo.Extension))
                    {
                        encryptionTime = CypherFile(file, destFile);
                    }
                    else
                    {
                        historyStopwatch.Start();

                    File.Copy(file, destFile, true);
                        error = SaveFileWithOverWrite(file, destFile);

                        historyStopwatch.Stop();
                    }
                    fileTransfered++;

                    //Write logs
                    progressLog.SourceFile = file;
                    progressLog.TargetFile = destFile;
                    progressLog.TotalFilesRemaining = fileToTranfer - fileTransfered;
                    progressLog.Progression = 100 * fileTransfered / fileToTranfer;
                    progressLog.SaveLog(_id);

                    historyLog.SourceFile = file;
                    historyLog.TargetFile = destFile;
                    historyLog.FileSize = (ulong)fileInfo.Length;
                    historyLog.TransferTime = historyStopwatch.Elapsed.TotalMilliseconds;
                    historyLog.SaveLog();
                }
                catch (FileNotFoundException FileE)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.SourceFile = file;
                    historyLog.TargetFile = destFile;
                    historyLog.FileSize = 0;
                    historyLog.TransferTime = -1;
                    historyLog.Error = FileE.ToString();
                    historyLog.SaveLog();

                    error = true;
                    break;
                }
                catch (Exception e)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.SourceFile = file;
                    historyLog.TargetFile = destFile;
                    historyLog.FileSize = 0;
                    historyLog.TransferTime = -1;
                    historyLog.Error = e.ToString();
                    historyLog.SaveLog();

                    error = true;
            progressLog.ResetProgressLog(_id);
            }

            //Reset progressLog
            progressLog.ResetProgressLog(_id);

            return error;
        }




            /// <summary> Save all differents files between _sourceDirectory and _destDirectory to _destDirectory.</summary>
            /// <remarks>This method ignores deleted files.</remarks>
            private bool DoDifferentialSave()
            {
                bool error = false;

            int encryptionTImeResult = 0;

            String[] files = FindFilesForDifferentialSave(_sourceDirectory);

            //Creation of all sub directories
            foreach (string path in Directory.GetDirectories(_sourceDirectory, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(path.Replace(_sourceDirectory, _destinationDirectory));
            }

            int fileTransfered = 0;                 //Incease each file transfered
            int fileToTranfer = files.Length;       //Ammount of file to transfer
            long sizeTotal = TotalFileSize(files);

            Stopwatch historyStopwatch = new Stopwatch();
            ProgressLog progressLog = new ProgressLog(_label, "", "", "ACTIVE", fileToTranfer, sizeTotal, fileToTranfer - fileTransfered);
            HistoryLog historyLog = new HistoryLog(_label, "", "", 0, 0, 0);

            foreach (String file in files)
            {
                try
                {
                    // Creation of the destFile
                    string destFile = file.Replace(_sourceDirectory, _destinationDirectory);
                    int encryptionTime;

                    FileInfo fileInfo = new FileInfo(file);
                    if (_extensionList.Contains(fileInfo.Extension))
                    {
                        encryptionTime = CypherFile(file, destFile);
                    }
                    else
                    {
                        historyStopwatch.Reset();
                        historyStopwatch.Start();

                    File.Copy(file, destFile, true);
                    historyStopwatch.Stop();
                        error = SaveFileWithOverWrite(file, destFile);

                        historyStopwatch.Stop();
                    }

                    fileTransfered++;

                    progressLog.SourceFile = file;
                    progressLog.TargetFile = destFile;
                    progressLog.TotalFilesRemaining = fileToTranfer - fileTransfered;
                    progressLog.Progression = 100 * fileTransfered / fileToTranfer;
                    progressLog.SaveLog(_id);

                    historyLog.SourceFile = file;
                    historyLog.TargetFile = destFile;
                    historyLog.FileSize = (ulong)fileInfo.Length;
                    historyLog.TransferTime = historyStopwatch.Elapsed.TotalMilliseconds;
                    historyLog.SaveLog();
                }
                catch (FileNotFoundException fileE)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.SourceFile = file;
                    historyLog.TargetFile = destFile;
                    historyLog.FileSize = 0;
                    historyLog.TransferTime = -1;
                    historyLog.Error = fileE.ToString();
                    historyLog.SaveLog();

                    error = true;
                    break;
                }
                catch (Exception e)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.SourceFile = file;
                    historyLog.TargetFile = destFile;
                    historyLog.FileSize = 0;
                    historyLog.TransferTime = -1;
                    historyLog.Error = e.ToString();
                    historyLog.SaveLog();

                    error = true;
                    break;
                }
            }

            //Reset progressLog
            progressLog.ResetProgressLog(_id);

            return error;
        }


        private void DeleteExcessFiles()
        {
            string[] files = Directory.GetFiles(_sourceDirectory, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {

            }
        }


        private long TotalFileSize(String[] files)
        {
            long totalSize = 0;

            foreach (String file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                totalSize += fileInfo.Length;
            }

            return totalSize;
        }



        private String[] FindFilesForDifferentialSave(String directory)
        {
            List<String> filesToSave = new List<string>();
            String[] filesInDirectory = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

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


        private int CypherFile(String sourceFile, String destFile)
        {
            Process process = new Process();
            int time;

            process.StartInfo.FileName = @"C:\EasySave\CryptoSoft\bin\Debug\netcoreapp3.1\CryptoSoft.exe";
            process.StartInfo.Arguments = sourceFile + " " + destFile;
            process.StartInfo.RedirectStandardOutput = true;
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés)
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                _disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~JobBackup()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}