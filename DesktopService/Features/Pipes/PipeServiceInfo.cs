using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Pipes
{
    public class PipeServiceInfo : IPipeServiceInfo
    {
        private readonly SharedConfigurations.DesktopService.Models.PipeConfiguration _pipeSettings;
        private readonly ILogger<PipeServiceInfo> _logger;

        public PipeServiceInfo(ILoggerFactory loggerFactory, IOptions<SharedConfigurations.DesktopService.Models.PipeConfiguration> pipeSettings)
        {
            _logger = loggerFactory.CreateLogger<PipeServiceInfo>();
            _pipeSettings = pipeSettings.Value;
        }

        public void ShowInfoWindow(bool forceStart = false)
        {
            if (_pipeSettings.ShowServiceInfoOnStart || forceStart)
            {
                if (!string.IsNullOrWhiteSpace(_pipeSettings.RediscoveryDesktopHubPath))
                {
                    if (System.IO.File.Exists(_pipeSettings.RediscoveryDesktopHubPath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = _pipeSettings.RediscoveryDesktopHubPath,
                            Arguments = $"--serviceinfo:{Program.HostIpAddress}"
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"Could not find Rediscovery Hub application file @{_pipeSettings.RediscoveryDesktopHubPath}");
                    }
                }
            }
        }
    }
}
