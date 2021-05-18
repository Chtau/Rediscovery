using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal abstract class BaseListener
    {
        private System.Threading.Thread listenThread;
        private readonly IProtocolLogger _logger;
        private readonly string threadName = $"Thread";

        private Setting setting;
        private bool working = false;

        public BaseListener(IProtocolLogger protocolLogger = null, string threadName = null)
        {
            _logger = protocolLogger ?? new ProtocolLogger();
            if (!string.IsNullOrWhiteSpace(threadName))
                this.threadName = threadName;
            OnInitThread();
        }

        public virtual void Initialize(Setting setting)
        {
            this.setting = setting;
        }

        public virtual bool Start()
        {
            try
            {
                working = true;
                listenThread.Start();
                return true;
            }
            catch (System.Threading.ThreadStateException tsEx)
            {
                _logger.Warning(tsEx);
                OnInitThread();
                try
                {
                    working = true;
                    listenThread.Start();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        public virtual bool Stop()
        {
            try
            {
                working = false;
                listenThread?.Abort();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        public virtual void OnBeforeDoWork()
        {

        }

        public virtual void OnDoWork()
        {

        }

        public virtual void OnBeforeRestartWorker()
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
                        OnBeforeDoWork();
                        while (working)
                        {
                            OnDoWork();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        // if we reach this point we need to restart
                        OnBeforeRestartWorker();
                        Start();
                    }
                })
                {
                    Name = $"{threadName}_{DateTime.Today.Ticks}"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
