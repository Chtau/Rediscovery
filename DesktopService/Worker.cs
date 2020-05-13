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

namespace DesktopService
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly Features.Configuration.IDistributeConfig _distributeConfig;
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;
        private readonly ILogger<Worker> _logger;

        public Worker(
            Features.Configuration.IDistributeConfig distributeConfig,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions,
            ILoggerFactory loggerFactory)
        {
            _distributeConfig = distributeConfig;
            _appSettings = appOptions.Value;
            _logger = loggerFactory.CreateLogger<Worker>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // check exec firewall rule
            if (!string.IsNullOrWhiteSpace(_appSettings.FirewallRuleName))
            {
                if (SharedFeatureFunctions.FirewallRule.RuleExists(_appSettings.FirewallRuleName) != SharedFeatureFunctions.FirewallRule.RuleState.True)
                {
                    var appPath = Process.GetCurrentProcess().MainModule.FileName;
                    var result = SharedFeatureFunctions.FirewallRule.RuleCreate(_appSettings.FirewallRuleName, appPath);
                    if (result != SharedFeatureFunctions.FirewallRule.RuleState.True)
                    {
                        if (result == SharedFeatureFunctions.FirewallRule.RuleState.False)
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
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
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
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
