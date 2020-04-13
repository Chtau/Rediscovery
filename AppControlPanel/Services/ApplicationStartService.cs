using SharedConfigurations.AppControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace AppControlPanel.Services
{
    public class ApplicationStartService : IApplicationStartService
    {
        public bool Start(AppModel appViewModel)
        {
            if (!string.IsNullOrWhiteSpace(appViewModel.ExecuteableName))
            {
                string path = null;
                if (!string.IsNullOrWhiteSpace(appViewModel.SearchDirectory) && System.IO.Directory.Exists(appViewModel.SearchDirectory))
                {
                    var foundPaths = System.IO.Directory.GetFiles(appViewModel.SearchDirectory, appViewModel.ExecuteableName, System.IO.SearchOption.AllDirectories);
                    if (foundPaths?.Length > 0)
                        path = foundPaths[0];
                }
                else
                {
                    // search in parent and all child folders from the parent
                    var parentDirInfo = System.IO.Directory.GetParent(GetApplicationFolder());
                    var foundPaths = System.IO.Directory.GetFiles(parentDirInfo.FullName, appViewModel.ExecuteableName, System.IO.SearchOption.AllDirectories);
                    if (foundPaths?.Length > 0)
                        path = foundPaths[0];
                }

                if (!string.IsNullOrWhiteSpace(path))
                {
                    ProcessRun(path, appViewModel.ExecuteArguments, null, appViewModel.RunAs);
                }
            }
            return false;
        }

        private string GetApplicationFolder()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        private bool ProcessRun(string filePath, string parameters, Action exitCallback, string runAs = null)
        {
            var SelfProc = new ProcessStartInfo
            {
                UseShellExecute = true,
                //WorkingDirectory = Environment.CurrentDirectory,
                FileName = filePath,
                Arguments = parameters
            };
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
    }
}
