using AppControlPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppControlPanel.Services
{
    public interface IApplicationWatchService
    {
        void Watch(AppViewModel appViewModel, Action<ViewModels.AppViewModel.LaunchState, int?> callback);
    }
}
