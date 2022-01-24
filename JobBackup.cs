using System;
using System.Diagnostics;

namespace EasySave
{
    public class JobBackup
    {
        private String _label;
        private String _sourceDirectory;
        private String _destinationDirectory;
        private Boolean _isDifferential;

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

            if (System.IO.Directory.Exists(_sourceDirectory))
            {
                string[] files = System.IO.Directory.GetFiles(_sourceDirectory);
                int counter = 0;

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(_destinationDirectory, fileName);
                    //The overwrite parameter is the "true" below
                    System.IO.File.Copy(s, destFile, true);
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

        }


        private String[] FindFilesForDifferentialSave()
        {
            String[] files = ["fichier"];


            return files;
        }
    }
}