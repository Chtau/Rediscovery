using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.ControlPanel.Services
{
    public interface IApplicationStartService
    {
        ViewModels.AppViewModel.LaunchState Start(SharedConfigurations.AppControlPanel.Models.AppModel appViewModel, Action<int> processIdCallback = null);
    }
}
