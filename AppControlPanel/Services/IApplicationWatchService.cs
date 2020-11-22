using Rediscovery.Client.App.ControlPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.ControlPanel.Services
{
    public interface IApplicationWatchService
    {
        void Watch(AppViewModel appViewModel, Action<ViewModels.AppViewModel.LaunchState, int?> callback);
    }
}
