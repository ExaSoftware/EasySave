using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave
{
    public static class Tools
    {
        public static Boolean CompareLists<T>(IList<T> List1, IList<T> List2)
        {
            return List1.Equals(List2)? true : false;
        }
    }
}
