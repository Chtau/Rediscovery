using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Pipes
{
    public class PipeIncomingConnection : IPipeIncomingConnection
    {
        private const string RediscoveryHub = "rediscoveryhub";
        private readonly IPCPipe.IPipeClient _pipeClient;
        private readonly ILogger<PipeIncomingConnection> _logger;
        private readonly SharedConfigurations.DesktopService.Models.PipeConfiguration _pipeSettings;

        public PipeIncomingConnection(IPCPipe.IPipeClient pipeClient, ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.PipeConfiguration> pipeSettings)
        {
            _pipeClient = pipeClient;
            _logger = loggerFactory.CreateLogger<PipeIncomingConnection>();
            _pipeSettings = pipeSettings.Value;
        }

        public async Task ShowCode(string code, string device, DateTime validTill)
        {
            try
            {
                if (_pipeClient.TryConnect(RediscoveryHub))
                {
                    var infoData = Newtonsoft.Json.JsonConvert.SerializeObject(new SharedCoreModels.IncomingConnectionInfo
                    {
                        Code = code,
                        Device = device,
                        ValidTill = validTill
                    });
                    _pipeClient.Send(RediscoveryHub, infoData);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(_pipeSettings.RediscoveryDesktopHubPath))
                    {
                        if (System.IO.File.Exists(_pipeSettings.RediscoveryDesktopHubPath))
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = _pipeSettings.RediscoveryDesktopHubPath,
                                Arguments = $"--code:{code} --device:{device} --valid:{validTill.Ticks}"
                            });
                        } else
                        {
                            _logger.LogWarning($"Could not find Rediscovery Hub application file @{_pipeSettings.RediscoveryDesktopHubPath}");
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
