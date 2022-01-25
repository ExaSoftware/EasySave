using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave
{
    public static class JobBackUpModel
    {
        public static void SaveFileWithOverWrite(string file, string destFile)
        {
            System.IO.File.Copy(file, destFile, true);
        }

        public static void SaveFileWithoutOverWrite(string file, string destFile)
        {s
            System.IO.File.Copy(file, destFile, false);
        }
    }
}
