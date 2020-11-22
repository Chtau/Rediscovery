using Rediscovery.Client.App.ControlPanel.ViewModels;
using SharedConfigurations.AppControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace Rediscovery.Client.App.ControlPanel.Services
{
    public class ApplicationWatchService : IApplicationWatchService
    {
        public void Watch(AppViewModel appViewModel, Action<AppViewModel.LaunchState, int?> callback)
        {
            if (appViewModel.AppModel == null)
            {
                callback?.Invoke(AppViewModel.LaunchState.Error, null);
                return;
            }
            string name = null;
            string tmpName = appViewModel.AppModel.ExecuteableName;
            if (!string.IsNullOrWhiteSpace(appViewModel.AppModel.ProcessName))
                tmpName = appViewModel.AppModel.ProcessName;
            if (tmpName.Contains('.'))
                name = tmpName.Substring(0, tmpName.LastIndexOf('.'));
            else
                name = tmpName;
            Process prc = Process.GetProcessesByName(name).FirstOrDefault();
            if (prc != null)
            {
                callback?.Invoke(AppViewModel.LaunchState.Running, prc.Id);
            } else
            {
                if (appViewModel.AdditionalProcesses.Count > 0)
                {
                    bool running = false;
                    List<int> processIdsToRemove = new List<int>();
                    foreach (var item in appViewModel.AdditionalProcesses)
                    {
                        try
                        {
                            Process prcTmp = Process.GetProcessById(item);
                            if (prcTmp != null)
                            {
                                callback?.Invoke(AppViewModel.LaunchState.Running, prcTmp.Id);
                                running = true;
                                break;
                            }
                        } catch (ArgumentException)
                        {
                            // if the process by id throws the argument exception the process no longer exists
                            processIdsToRemove.Add(item);
                        } catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Print("Process Watch additional Processes Exception:" + ex.ToString());
                        }
                    }
                    if (processIdsToRemove.Count > 0)
                    {
                        foreach (var item in processIdsToRemove)
                        {
                            appViewModel.AdditionalProcesses.Remove(item);
                        }
                    }
                    if (!running)
                    {
                        callback?.Invoke(AppViewModel.LaunchState.NotRunning, null);
                    }
                } else
                    callback?.Invoke(AppViewModel.LaunchState.NotRunning, null);
            }
        }
    }
}
