using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace RemoteInterface
{
    delegate void Del(JobBackup jobBackup);

    class MainViewModel
    {
        //Attributes
        private List<JobBackup> _listOfJobBackup;

        //Define getter / setter
        public List<JobBackup> ListOfJobBackup { get => _listOfJobBackup; set => _listOfJobBackup = value; }

        /// <summary>
        /// Constructor of MainViewModel.
        /// </summary>
        public MainViewModel()
        {
        }

        //Instanciate the delegate
        readonly Del Execute = delegate (JobBackup jobBackup)
        {
        };

        /// <summary>
        /// Instanciate a thread and execute the jobBackup in this thread.
        /// </summary>
        /// <param name="jobBackup">The JobBackup to execute.</param>
        public void ExecuteOne(JobBackup jobBackup)
        {
        }

        /// <summary>
        /// Execute all the list of JobBackup with the ExecuteOne method.
        /// </summary>
        /// <remarks>Threads are executed one by one, in the order of the list.</remarks>
        public void ExecuteAll()
        {
        }

    }
}