using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Client.App.Service
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly Features.Configuration.IDistributeConfig _distributeConfig;
        private readonly Rediscovery.Shared.Configurations.Service.Models.AppConfiguration _appSettings;
        private readonly ILogger<Worker> _logger;
        private readonly Features.DeviceFeature.IFeatureService _featureService;
        private readonly Services.IStaticResources _staticResources;

        public Worker(
            Features.Configuration.IDistributeConfig distributeConfig,
            IOptions<Rediscovery.Shared.Configurations.Service.Models.AppConfiguration> appOptions,
            Features.DeviceFeature.IFeatureService featureService,
            ILoggerFactory loggerFactory,
            Services.IStaticResources staticResources)
        {
            _distributeConfig = distributeConfig;
            _appSettings = appOptions.Value;
            _featureService = featureService;
            _staticResources = staticResources;
            _logger = loggerFactory.CreateLogger<Worker>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // check exec firewall rule
            if (!string.IsNullOrWhiteSpace(_appSettings.FirewallRuleName))
            {
                if (Rediscovery.Feature.Shared.Functions.FirewallRule.RuleExists(_appSettings.FirewallRuleName) != Rediscovery.Feature.Shared.Functions.FirewallRule.RuleState.True)
                {
                    var appPath = Process.GetCurrentProcess().MainModule.FileName;
                    var result = Rediscovery.Feature.Shared.Functions.FirewallRule.RuleCreate(_appSettings.FirewallRuleName, appPath);
                    if (result != Rediscovery.Feature.Shared.Functions.FirewallRule.RuleState.True)
                    {
                        if (result == Rediscovery.Feature.Shared.Functions.FirewallRule.RuleState.False)
                            _logger.LogError($"Could not add Firewall rule, can't access Network traffic if a Firewall is active");
                        else
                            _logger.LogError($"Could not add Firewall rule, can't access Network traffic if a Firewall is active (required Administration privileges)");
                    } else
                    {
                        _logger.LogInformation("Firewall rule was added");
                    }
                } else
                {
                    _logger.LogInformation("Firewall rule was found");
                }
            }

            _distributeConfig.Share();

            _logger.LogInformation($"Service Worker started");
            _logger.LogInformation($"Loaded with Resources:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(_staticResources, Newtonsoft.Json.Formatting.Indented)}\r\n");

            // the Task.Run leads to a thread starvation
            /*Task.Run(() =>
            {
                Task.Delay(2000);
                //_pipeIncomingConnection.ShowCode("999666", "internal-test");
                do
                {
                    //Console.WriteLine($"{DateTime.Now}");
                    Task.Delay(500);
                } while (!cancellationToken.IsCancellationRequested);
            });*/
            //_featureService.GetFeaturesManifest();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                disposedValue = true;
            }
        }

        // ~Worker()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
