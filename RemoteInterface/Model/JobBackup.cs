using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace RemoteInterface
{
    /// <summary>
    ///  JobBackup is a class. It allows you to save files from a directory to another one with differents methods.
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
        private String[] _extensionList;
        private bool _disposedValue;

        // Properties
        public string SourceDirectory { get => _sourceDirectory; set => _sourceDirectory = value; }
        public string DestinationDirectory { get => _destinationDirectory; set => _destinationDirectory = value; }
        public bool IsDifferential { get => _isDifferential; set => _isDifferential = value; }
        public string Label { get => _label; set => _label = value; }
        public int Id { get => _id; set => _id = value; }

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
        ///  Execute the save according to attributes.
        ///  </summary>
        ///  <remarks>Use it whether differential or not.</remarks>
        public void Execute()
        {
        }

        ///  <summary>
        ///  Save all files from _sourceDirectory to _destDirectory.
        ///  </summary>
        ///  <remarks>This method delete all the destination directory before saving files</remarks>
        private void SaveAllFiles()
        {
        }

        /// <summary> 
        /// Save all differents files between _sourceDirectory and _destDirectory to _destDirectory.
        /// </summary>
        /// <remarks>This method ignores deleted files.</remarks>
        private void DoDifferentialSave()
        {
            
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
        /// The GarbageCollector is not called. 
        /// </summary>
    }
}