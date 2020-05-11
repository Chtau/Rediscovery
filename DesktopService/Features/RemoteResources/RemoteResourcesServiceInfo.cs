using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.RemoteResources
{
    public class RemoteResourcesServiceInfo : IRemoteResourcesServiceInfo
    {
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly ILogger<RemoteResourcesServiceInfo> _logger;
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;

        public RemoteResourcesServiceInfo(ILoggerFactory loggerFactory, IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions)
        {
            _logger = loggerFactory.CreateLogger<RemoteResourcesServiceInfo>();
            _remoteResourceSettings = remoteResourceSettings.Value;
            _appSettings = appOptions.Value;
        }

        public void ShowInfoWindow(bool forceStart = false)
        {
            if (_remoteResourceSettings.ShowServiceInfoOnStart || forceStart)
            {
                // TODO: refactor ShowInfoWindow
                /*if (ActiveDesktopInfoHandler.ConnectionIds.Count > 0)
                {
                    _hubContext.Clients.Group(DesktopInfoHubRemoteResourceHub.GroupName).SendAsync("ApplicationInfo", SharedCommandArguments.Hub.Arguments.ServiceInfoStart);
                } else if (!string.IsNullOrWhiteSpace(_remoteResourceSettings.RediscoveryDesktopInfoHubPath))
                {
                    if (System.IO.File.Exists(_remoteResourceSettings.RediscoveryDesktopInfoHubPath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = _remoteResourceSettings.RediscoveryDesktopInfoHubPath,
                            Arguments = SharedCommandArguments.Hub.Arguments.ServiceInfoStart
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"Could not find Rediscovery Hub application file @{_remoteResourceSettings.RediscoveryDesktopInfoHubPath}");
                    }
                }*/
            }
        }
    }
}
