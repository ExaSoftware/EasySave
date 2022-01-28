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
    public class JobBackup
    {
        // Attributes
        private String _label;
        private String _sourceDirectory;
        private String _destinationDirectory;
        private Boolean _isDifferential;
        private int _id;

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
        public bool Execute()
        {
            bool error = false;
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
                catch(DirectoryNotFoundException)
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

            string[] files = Directory.GetFiles(_sourceDirectory);

            int fileTransfered = 0;                 //Incease each file transfered
            int fileToTranfer = files.Length;       //Ammount of file to transfer
            long sizeTotal = totalFileSize(files);

            // Setup stopwatch
            Stopwatch historyStopwatch = new Stopwatch();
            ProgressLog progressLog = new ProgressLog(_label, "", "", "ACTIVE", fileToTranfer, sizeTotal, fileToTranfer - fileTransfered);
            HistoryLog historyLog = new HistoryLog(_label, "", "", 0, 0);

            // Copy the files and overwrite destination files if they already exist.
            foreach (string file in files)
            {
                try
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);
                    FileInfo fileInfo = new FileInfo(file);

                    historyStopwatch.Reset();
                    historyStopwatch.Start();

                    error = SaveFileWithOverWrite(file, destFile);

                    historyStopwatch.Stop();
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
                catch (FileNotFoundException)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.SourceFile = file;
                    historyLog.TargetFile = destFile;
                    historyLog.FileSize = 0;
                    historyLog.TransferTime = -1;
                    historyLog.SaveLog();

                    error = true;
                    break;
                }
            }

            //Reset progressLog
            progressLog.SourceFile = "";
            progressLog.TargetFile = "";
            progressLog.State = "END";
            progressLog.TotalFilesRemaining = 0;
            progressLog.TotalFilesToCopy = 0;
            progressLog.TotalFilesSize = 0;
            progressLog.TotalFilesRemaining = fileToTranfer - fileTransfered;
            progressLog.Progression = 0;
            progressLog.SaveLog(_id);

            return error;
        }




        /// <summary> Save all differents files between _sourceDirectory and _destDirectory to _destDirectory.</summary>
        /// <remarks>This method ignores deleted files.</remarks>
        private bool DoDifferentialSave()
        {
            bool error = false;

            String[] files = findFilesForDifferentialSave();

            int fileTransfered = 0;                 //Incease each file transfered
            int fileToTranfer = files.Length;       //Ammount of file to transfer
            long sizeTotal = totalFileSize(files);
            Stopwatch historyStopwatch = new Stopwatch();
            ProgressLog progressLog = new ProgressLog(_label, "", "", "ACTIVE", fileToTranfer, sizeTotal, fileToTranfer - fileTransfered);
            HistoryLog historyLog = new HistoryLog(_label, "", "", 0, 0);


            foreach (String file in files)
            {
                try
                {
                    // Creation of the destFile
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    FileInfo fileInfo = new FileInfo(file);
                    historyStopwatch.Reset();
                    historyStopwatch.Start();

                    error = SaveFileWithOverWrite(file, destFile);
                    historyStopwatch.Stop();
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
                catch (FileNotFoundException)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.SourceFile = file;
                    historyLog.TargetFile = destFile;
                    historyLog.FileSize = 0;
                    historyLog.TransferTime = -1;
                    historyLog.SaveLog();

                    error = true;
                    break;
                }
            }

            //Reset progressLog
            progressLog.SourceFile = "";
            progressLog.TargetFile = "";
            progressLog.State = "END";
            progressLog.TotalFilesRemaining = 0;
            progressLog.TotalFilesToCopy = 0;
            progressLog.TotalFilesSize = 0;
            progressLog.TotalFilesRemaining = fileToTranfer - fileTransfered;
            progressLog.Progression = 0;
            progressLog.SaveLog(_id);

            return error;
        }



        private long totalFileSize(String[] files)
        {
            long totalSize = 0;

            foreach (String file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                totalSize += fileInfo.Length;
            }

            return totalSize;
        }



        private String[] findFilesForDifferentialSave()
        {
            List<String> filesToSave = new List<string>();
            String[] filesInSourceDirectory = Directory.GetFiles(_sourceDirectory);

            foreach (String file in filesInSourceDirectory)
            {
                // Creation of the destFile
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(_destinationDirectory, fileName);

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

    }
}