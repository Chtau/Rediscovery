using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery
{
    public static class HtmlUIHelpers
    {
        public static string GetIndexFile(string directory)
        {
            if (!string.IsNullOrWhiteSpace(directory) && System.IO.Directory.Exists(directory))
            {
                // find start file
                string startFile = "";
                if (System.IO.File.Exists(System.IO.Path.Combine(directory, "Index.html")))
                    startFile = System.IO.Path.Combine(directory, "Index.html");
                else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "index.html")))
                    startFile = System.IO.Path.Combine(directory, "index.html");
                else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "default.html")))
                    startFile = System.IO.Path.Combine(directory, "default.html");
                else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "Default.html")))
                    startFile = System.IO.Path.Combine(directory, "Default.html");
                return startFile;
            }
            return null;
        }
    }
}
