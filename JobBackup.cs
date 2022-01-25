using System;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using static EasySave.JobBackUpModel;
using static EasySave.Tools;

namespace EasySave
{
    public class JobBackup
    {
        // Attributes
        private String _label;
        private String _sourceDirectory;
        private String _destinationDirectory;
        private Boolean _isDifferential;

        // Properties
        public string SourceDirectory { get => _sourceDirectory; set => _sourceDirectory = value; }
        public string DestinationDirectory { get => _destinationDirectory; set => _destinationDirectory = value; }
        public bool IsDifferential { get => _isDifferential; set => _isDifferential = value; }
        public string Label { get => _label; set => _label = value; }

        // Default constructor to use in serialize
        public JobBackup() { }

        // Constructor with all attributes definition
        public JobBackup(String label, String sourceDirectory, String destinationDirectory, Boolean isDifferential)
        {
            this._label = label;
            this._sourceDirectory = sourceDirectory;
            this._destinationDirectory = destinationDirectory;
            this._isDifferential = isDifferential;
        }


        public void Execute()
        {
            if (_isDifferential)
            {
                DoDifferentialSave();
            }

            else
            {
                SaveAllFiles();
            }
        }


        // Save all files from _sourceDirectory to _destDirectory
        private string SaveAllFiles()
        {
            // Setup stopwatch
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (Directory.Exists(_sourceDirectory))
            {
                string[] files = Directory.GetFiles(_sourceDirectory);
                int counter = 0;

                // Copy the files and overwrite destination files if they already exist.
                foreach (string file in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_destinationDirectory, fileName);
                    
                    //The overwrite parameter is the "true" below
                    File.Copy(file, destFile, true);
                    Console.WriteLine("{0} file mooved to: {1}", fileName, _destinationDirectory);
                    counter++;
                }

                Console.WriteLine("{0} files mooved to: {1}", counter, _destinationDirectory);
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds.ToString();
        }


        private void DoDifferentialSave()
        {
            String[] files = Directory.GetFiles(_sourceDirectory);

            foreach ( String file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(_destinationDirectory, fileName);

                //Add hashcode comparaison
                byte[] inComingFileHash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(file));

                if (File.Exists(destFile))
                {
                    byte[] destinationFileHash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(destFile));
                 
                    if (CompareLists(destinationFileHash, inComingFileHash))
                    {
                        SaveFileWithOverWrite(file, destFile);
                    }
                }
            }
        }


    }
}