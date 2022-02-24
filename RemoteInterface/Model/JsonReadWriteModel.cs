using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RemoteInterface
{
    /// <summary>
    /// This class save history and progress logs in this path : 'c:\EasySave\Logs' and create the directory '\Logs if the repository doesn't exist
    /// </summary>
    public class JsonReadWriteModel
    {
        /// <summary>
        /// Method which read a json file and convert it into a list of list of JobBackup
        /// </summary>
        /// <returns>The list of JobBackup associate to the json file</returns>
        public ProgressLog[] ReadProgressLog(string message)
        {
            //return JsonConvert.DeserializeObject<List<ProgressLog>>(message);
            return JsonConvert.DeserializeObject<ProgressLog[]>(message);
        }
        public string PrepareLogForRemote(ProgressLog log)
        {
            string jsonStringLog = JsonConvert.SerializeObject(log, Newtonsoft.Json.Formatting.Indented);
            return jsonStringLog;
        }

    }
}