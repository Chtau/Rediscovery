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
        public ViewModels.AppViewModel.LaunchState Start(AppModel appViewModel)
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
                    var parentDirInfo = System.IO.Directory.GetParent(Shared.GetApplicationFolder());
                    var foundPaths = System.IO.Directory.GetFiles(parentDirInfo.FullName, appViewModel.ExecuteableName, System.IO.SearchOption.AllDirectories);
                    if (foundPaths?.Length > 0)
                        path = foundPaths[0];
                }

                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (Shared.ProcessRun(path, appViewModel.ExecuteArguments, null, appViewModel.RunAs, appViewModel.HideShell))
                        return ViewModels.AppViewModel.LaunchState.Starting;
                    else
                        return ViewModels.AppViewModel.LaunchState.ErrorStarting;
                } else
                {
                    return ViewModels.AppViewModel.LaunchState.NotFound;
                }
            }
            return ViewModels.AppViewModel.LaunchState.Error;
        }
    }
}
