using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
