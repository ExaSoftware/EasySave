using EasySave.Object;
using EasySave.Util;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace EasySave
{
    public class Program
    {
        static void Main(string[] args)
        {
            MainViewModel menu= new MainViewModel();
            menu.Start();
        }
    }
}