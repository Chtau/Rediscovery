using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.RemoteResources
{
    public class RemoteResourcesIncomingConnection : IRemoteResourcesIncomingConnection
    {
        private readonly ILogger<RemoteResourcesIncomingConnection> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly IHubContext<DesktopInfoHubRemoteResourceHub> _hubContext;

        public RemoteResourcesIncomingConnection(ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            IHubContext<DesktopInfoHubRemoteResourceHub> hubContext)
        {
            _logger = loggerFactory.CreateLogger<RemoteResourcesIncomingConnection>();
            _remoteResourceSettings = remoteResourceSettings.Value;
            _hubContext = hubContext;
        }

        public async Task ShowCode(string code, string device, DateTime validTill)
        {
            try
            {
                if (ActiveDesktopInfoHandler.ConnectionIds.Count > 0)
                {
                    var infoData = new SharedCoreModels.IncomingConnectionInfo
                    {
                        Code = code,
                        Device = device,
                        ValidTill = validTill
                    };
                    await _hubContext.Clients.Group(DesktopInfoHubRemoteResourceHub.GroupName).SendAsync("NewValidationCode", infoData);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(_remoteResourceSettings.RediscoveryDesktopInfoHubPath))
                    {
                        if (System.IO.File.Exists(_remoteResourceSettings.RediscoveryDesktopInfoHubPath))
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = _remoteResourceSettings.RediscoveryDesktopInfoHubPath,
                                Arguments = $"{SharedCommandArguments.Hub.Arguments.CodeArgStart}{code} {SharedCommandArguments.Hub.Arguments.DeviceArgStart}{device} {SharedCommandArguments.Hub.Arguments.ValidArgStart}{validTill.Ticks}"
                            });
                        } else
                        {
                            _logger.LogWarning($"Could not find Rediscovery Hub application file @{_remoteResourceSettings.RediscoveryDesktopInfoHubPath}");
                        }
                    }
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
