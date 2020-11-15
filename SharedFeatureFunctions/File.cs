using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SharedFeatureFunctions
{
    public static class File
    {
        public static void OpenWithDefaultProgram(string path)
        {
            System.Diagnostics.Process fileopener = new System.Diagnostics.Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        public static void OpenDirectory(string directory, string filename = null, string extension = null)
        {
            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (!extension.StartsWith("."))
                    extension = "." + extension;
            }
            string filePath;
            if (!string.IsNullOrWhiteSpace(filename))
            {
                if (!string.IsNullOrWhiteSpace(extension))
                    filePath = System.IO.Path.Combine(directory, filename + extension);
                else
                    filePath = System.IO.Path.Combine(directory, filename);
            }
            else
            {
                filePath = directory;
            }
            string argument = "/select, \"" + filePath + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        public static string GetUserFolder(string appName)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!string.IsNullOrWhiteSpace(appName))
                folder = Path.Combine(folder, appName);
            return GetFolder(folder);
        }

        public static string GetApplicationFolder()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static string GetFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return folder;
        }

        public static bool IsValidFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
