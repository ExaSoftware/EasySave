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
        private List<String> _extensionList = new List<string> { "pdf", "xlsx" };

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
        public bool Execute(string buisnessSoftwareName)
        {
            bool error = false;
            Process[] procName = Process.GetProcessesByName(buisnessSoftwareName);
            if (procName.Length != 0)
            {
                return true;
            }
            else
            {
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
        }

        ///  <summary>Save all files from _sourceDirectory to _destDirectory.</summary>
        ///  <remarks>This method ignores deleted files.</remarks>
        private bool SaveAllFiles()
        {
            bool error = false;
            int encryptionTimeResult = 0;

            //Creation of all sub directories
            foreach (string path in Directory.GetDirectories(_sourceDirectory, "*", SearchOption.AllDirectories))
            {
                Console.WriteLine(path);
                Console.WriteLine(_destinationDirectory);
                Console.WriteLine(_sourceDirectory);
                Console.WriteLine(path.Replace(_sourceDirectory, _destinationDirectory));
                Directory.CreateDirectory(path.Replace(_sourceDirectory, _destinationDirectory));
            }

            string[] files = Directory.GetFiles(_sourceDirectory, "*", SearchOption.AllDirectories);
            int fileTransfered = 0;                 //Incease each file transfered
            int fileToTranfer = files.Length;       //Ammount of file to transfer
            long sizeTotal = TotalFileSize(files);

            // Setup objects
            Stopwatch historyStopwatch = new Stopwatch();
            ProgressLog progressLog = new ProgressLog(_label, "", "", "ACTIVE", fileToTranfer, sizeTotal, fileToTranfer - fileTransfered);
            HistoryLog historyLog = new HistoryLog(_label, "", "", 0, 0, 0);

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
                        historyStopwatch.Stop();
                    }
                    fileTransfered++;

                    //Write logs
                    progressLog.Fill(file, destFile, (fileToTranfer - fileTransfered), (100 * fileTransfered / fileToTranfer), _id);

                    historyLog.Fill(file, destFile, fileInfo.Length, historyStopwatch.Elapsed.TotalMilliseconds, "", encryptionTimeResult);
                }
                catch (FileNotFoundException FileE)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.Fill(file, destFile, 0, -1, FileE.ToString(), encryptionTimeResult);

                    error = true;
                    break;
                }
                catch (Exception e)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.Fill(file, destFile, 0, -1, e.ToString(), encryptionTimeResult);

                    error = true;
                    break;
                }
            }

            //Reset progressLog
            progressLog.Reset(_id);
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

                    progressLog.Fill(file, destFile, (fileToTranfer - fileTransfered), (100 * fileTransfered / fileToTranfer), _id);

                    historyLog.Fill(file, destFile, fileInfo.Length, historyStopwatch.Elapsed.TotalMilliseconds, "", encryptionTImeResult);
                }
                catch (FileNotFoundException fileE)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.Fill(file, destFile, 0, -1, fileE.ToString(), encryptionTImeResult);

                    error = true;
                    break;
                }
                catch (Exception e)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);

                    historyLog.Fill(file, destFile, 0, -1, e.ToString(), encryptionTImeResult);

                    error = true;
                    break;
                }
            }



            //Reset progressLog
            progressLog.Reset(_id);

            return error;
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

    }
}