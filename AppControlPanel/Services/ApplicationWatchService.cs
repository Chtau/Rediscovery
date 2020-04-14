using AppControlPanel.ViewModels;
using SharedConfigurations.AppControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace AppControlPanel.Services
{
    public class ApplicationWatchService : IApplicationWatchService
    {
        public void Watch(AppModel appViewModel, Action<AppViewModel.LaunchState, int?> callback)
        {
            string name = null;
            string tmpName = appViewModel.ExecuteableName;
            if (!string.IsNullOrWhiteSpace(appViewModel.ProcessName))
                tmpName = appViewModel.ProcessName;
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
                callback?.Invoke(AppViewModel.LaunchState.NotRunning, null);
            }
        }
    }
}
