using System;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "test.txt";
            string sourcePath = @"C:\Users\bryan\Documents\départ";
            string targetPath = @"C:\Users\bryan\Documents\arrivée";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);
        }
    }
}