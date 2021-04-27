using Rediscovery.Shared.Configurations.ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Rediscovery.Client.App.ControlPanel.Services
{
    public class ApplicationStartService : IApplicationStartService
    {
        public ViewModels.AppViewModel.LaunchState Start(AppModel appViewModel, Action<int> processIdCallback = null)
        {
            if (appViewModel.UseCommandLine.HasValue && appViewModel.UseCommandLine.Value)
            {
                if (!string.IsNullOrWhiteSpace(appViewModel.CommandLineCommand) && !string.IsNullOrWhiteSpace(appViewModel.CommandLineWorkingDirectory))
                {
                    if (System.IO.Directory.Exists(appViewModel.CommandLineWorkingDirectory))
                    {
                        if (Rediscovery.Feature.Shared.Functions.Process.CommandLine(appViewModel.CommandLineCommand, appViewModel.CommandLineWorkingDirectory ?? appViewModel.WorkingDirectory, null, appViewModel.RunAs, appViewModel.HideShell.HasValue ? appViewModel.HideShell.Value : false, processIdCallback))
                            return ViewModels.AppViewModel.LaunchState.Starting;
                        else
                            return ViewModels.AppViewModel.LaunchState.ErrorStarting;
                    } else
                    {
                        return ViewModels.AppViewModel.LaunchState.NotFound;
                    }
                }
            } else
            {
                if (!string.IsNullOrWhiteSpace(appViewModel.ExecuteableName))
                {
                    string path = null;
                    
                    if (!string.IsNullOrWhiteSpace(appViewModel.SearchDirectory))
                    {
                        string travelPath = appViewModel.SearchDirectory;
                        if (!System.IO.Directory.Exists(travelPath))
                        {
                            travelPath = Path.GetFullPath(appViewModel.SearchDirectory);
                            if (!System.IO.Directory.Exists(travelPath))
                            {
                                return ViewModels.AppViewModel.LaunchState.NotFound;
                            }
                        }
                        var foundPaths = System.IO.Directory.GetFiles(travelPath, appViewModel.ExecuteableName, System.IO.SearchOption.AllDirectories);
                        if (foundPaths?.Length > 0)
                            path = foundPaths[0];
                    }
                    else
                    {
                        // search in parent and all child folders from the parent
                        var parentDirInfo = System.IO.Directory.GetParent(Rediscovery.Feature.Shared.Functions.File.GetApplicationFolder());
                        var foundPaths = System.IO.Directory.GetFiles(parentDirInfo.FullName, appViewModel.ExecuteableName, System.IO.SearchOption.AllDirectories);
                        if (foundPaths?.Length > 0)
                            path = foundPaths[0];
                    }

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        if (Rediscovery.Feature.Shared.Functions.Process.Run(path, appViewModel.ExecuteArguments, null, appViewModel.RunAs, appViewModel.HideShell.HasValue ? appViewModel.HideShell.Value : false, appViewModel.WorkingDirectory, processIdCallback))
                            return ViewModels.AppViewModel.LaunchState.Starting;
                        else
                            return ViewModels.AppViewModel.LaunchState.ErrorStarting;
                    }
                    else
                    {
                        return ViewModels.AppViewModel.LaunchState.NotFound;
                    }
                }
            }
            return ViewModels.AppViewModel.LaunchState.Error;
        }
    }
}
