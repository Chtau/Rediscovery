using Rediscovery.Shared.Configurations.ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.ControlPanel.Services
{
    public interface IApplicationStartService
    {
        ViewModels.AppViewModel.LaunchState Start(AppModel appViewModel, Action<int> processIdCallback = null);
    }
}
