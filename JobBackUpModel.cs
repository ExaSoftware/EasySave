using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave
{
    public static class JobBackUpModel
    {
        public static bool SaveFileWithOverWrite(string file, string destFile)
        {
            try
            {
            System.IO.File.Copy(file, destFile, true);
                return false;
            }
            catch(Exception)
            {
                return true;
            }
        }

        public static bool SaveFileWithoutOverWrite(string file, string destFile)
        {
            try
            {
                System.IO.File.Copy(file, destFile, false);
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
