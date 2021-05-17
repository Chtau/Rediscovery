using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal abstract class BaseListener
    {
        private System.Threading.Thread listenThread;
        private readonly IProtocolLogger _logger;
        private string threadName = $"Thread_{DateTime.Today.Ticks}";

        public BaseListener(IProtocolLogger protocolLogger = null, string threadName = null)
        {
            _logger = protocolLogger ?? new ProtocolLogger();
            if (!string.IsNullOrWhiteSpace(threadName))
                this.threadName = threadName;
            OnInitThread();
        }

        public virtual void Start()
        {
            try
            {
                listenThread.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public virtual void Stop()
        {
            try
            {
                listenThread.Abort();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public virtual void OnDoWork()
        {

        }

        private void OnInitThread()
        {
            try
            {
                listenThread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            OnDoWork();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        // TODO: if we reach this point we need to restart
                    }
                })
                {
                    Name = threadName
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
