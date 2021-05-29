using System;
using System.IO;

namespace WhereIsForward
{
    class Utils
    {
        public static string PathToResource(string file)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", file);
        }
    }
}
