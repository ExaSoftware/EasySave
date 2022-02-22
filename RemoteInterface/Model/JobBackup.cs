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

namespace RemoteInterface
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
        private List<string> _bigFilesList = new List<string>();

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
            _isRunning = false;
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
            _isRunning = false;
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