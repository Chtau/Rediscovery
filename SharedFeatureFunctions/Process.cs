using System;
using System.Collections.Generic;
using System.Text;

namespace SharedFeatureFunctions
{
    public static class Process
    {
        public static bool Run(string filePath, string parameters, Action exitCallback, string runAs = null, bool hideShell = false, string workingDirectory = null, Action<int> processIdCallback = null)
        {
            var SelfProc = new System.Diagnostics.ProcessStartInfo
            {
                UseShellExecute = !hideShell,
                //WorkingDirectory = Environment.CurrentDirectory,
                FileName = filePath,
                Arguments = parameters,
                CreateNoWindow = hideShell,
            };
            if (hideShell)
            {
                SelfProc.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }
            if (!string.IsNullOrWhiteSpace(workingDirectory))
                SelfProc.WorkingDirectory = workingDirectory;
            if (!string.IsNullOrWhiteSpace(runAs))
                SelfProc.Verb = runAs;
            // use "runas" for admin rights
            try
            {
                var prc = System.Diagnostics.Process.Start(SelfProc);
                processIdCallback?.Invoke(prc.Id);
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

        public static bool CommandLine(string command, string workingDirectory, Action exitCallback, string runAs = null, bool hideShell = false, Action<int> processIdCallback = null)
        {
            if (!command.StartsWith("/C"))
                command = "/C " + command;
            return Run("cmd.exe", command, exitCallback, runAs, hideShell, workingDirectory, processIdCallback);
        }
    }
}
