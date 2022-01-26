﻿using System;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using static EasySave.JobBackUpModel;
using static EasySave.Tools;

namespace EasySave
{
    /// <summary>
    ///  JobBackup is a Picasso class. It allows you to save files from a directory to an other with differents methods.
    ///  
    ///  (String) Label: the name of the save
    ///  (String: Path format) SourceDirectory: the source directory
    ///  (String: Path format) DestinationDirectory: the destination directory
    ///  (Boolean) IsDifferential: Define if the save is differential or no
    ///  
    ///  JobBackup has to be instantiate with the default contructor or the complete one.
    ///  
    /// </summary>
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
        
        public void Reset()
        {
            _label = "";
            _sourceDirectory = "";
            _destinationDirectory = "";
            _isDifferential = false;
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
            // Get files from the source directory
            String[] files = Directory.GetFiles(_sourceDirectory);

            foreach ( String file in files)
            {
                // Creation of the destFile
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(_destinationDirectory, fileName);

                //Get the hashCode of source file
                byte[] inComingFileHash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(file));

                if (File.Exists(destFile))
                {
                    //Get the hashCode of the destFle
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