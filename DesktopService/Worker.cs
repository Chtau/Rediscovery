using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopService
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly Features.Pipes.IPipeIncomingConnection _pipeIncomingConnection;
        private readonly Features.Pipes.IPipeRepository _pipeRepository;
        private readonly Features.Pipes.IPipeServiceInfo _pipeServiceInfo;
        private readonly Features.Configuration.IDistributeConfig _distributeConfig;

        public Worker(Features.Pipes.IPipeIncomingConnection pipeIncomingConnection,
            Features.Pipes.IPipeRepository pipeRepository,
            Features.Pipes.IPipeServiceInfo pipeServiceInfo,
            Features.Configuration.IDistributeConfig distributeConfig)
        {
            _pipeIncomingConnection = pipeIncomingConnection;
            _pipeRepository = pipeRepository;
            _pipeServiceInfo = pipeServiceInfo;
            _distributeConfig = distributeConfig;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _pipeRepository.Init();
            _distributeConfig.Share();
            _pipeServiceInfo.ShowInfoWindow();

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
