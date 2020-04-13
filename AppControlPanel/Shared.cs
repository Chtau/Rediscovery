using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace AppControlPanel
{
    public static class Shared
    {
        public static string GetApplicationFolder()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static bool ProcessRun(string filePath, string parameters, Action exitCallback, string runAs = null, bool hideShell = false)
        {
            var SelfProc = new ProcessStartInfo
            {
                UseShellExecute = !hideShell,
                //WorkingDirectory = Environment.CurrentDirectory,
                FileName = filePath,
                Arguments = parameters,
                CreateNoWindow = hideShell,
            };
            if (hideShell)
            {
                SelfProc.WindowStyle = ProcessWindowStyle.Hidden;
            }
            if (!string.IsNullOrWhiteSpace(runAs))
                SelfProc.Verb = runAs;
            // use "runas" for admin rights
            try
            {
                var prc = Process.Start(SelfProc);
                prc.Exited += (object sender, EventArgs e) =>
                {
                    exitCallback?.Invoke();
                };
                return true;
            }
            catch
            {
                System.Diagnostics.Debug.Print("Unable to run process!" + Environment.NewLine);
                return false;
            }
        }

        public static void OpenWithDefaultProgram(string path)
        {
            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }
    }
}
