using System;
using System.Collections.Generic;
using System.Text;

namespace AppControlPanel.Services
{
    public interface IApplicationWatchService
    {
        void Watch(SharedConfigurations.AppControlPanel.Models.AppModel appViewModel, Action<ViewModels.AppViewModel.LaunchState, int?> callback);
    }
}
