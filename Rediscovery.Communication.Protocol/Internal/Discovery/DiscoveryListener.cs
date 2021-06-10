using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using Rediscovery.Communication.Protocol.Internal.Device;
using System.Threading;

namespace Rediscovery.Communication.Protocol.Internal.Discovery
{
    internal class DiscoveryListener
    {
        private readonly IProtocolLogger _logger;
        private readonly IDiscoveryPipeline _discoveryPipeline;
        private readonly IDeviceManager _deviceManager;

        private System.Threading.Thread listenThread;
        private readonly string threadName = $"Thread_{nameof(DiscoveryListener)}";

        private DiscoveryConfiguration configuration;
        private bool working = false;
        private TimeSpan deviceTimeoutOffset = TimeSpan.FromSeconds(10);
        private string currentIdentifier;
        private Socket socket;

        public DiscoveryListener(IProtocolLogger protocolLogger, 
            IDiscoveryPipeline discoveryPipeline,
            IDeviceManager deviceManager)
        {
            _logger = protocolLogger;
            _discoveryPipeline = discoveryPipeline;
            _deviceManager = deviceManager;
            OnInitThread();
        }

        public void SetIdentifier(string identifier) => currentIdentifier = identifier;

        public void Initialize(DiscoveryConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Start()
        {
            try
            {
                if (configuration.ListenerDeactivated)
                    return true;
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

        public void Stop()
        {
            try
            {
                working = false;
                if (socket != null)
                {
                    try
                    {
                        socket.Close();
                        socket.Dispose();
                    } catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
                listenThread?.Abort();
            }
            catch (PlatformNotSupportedException) { }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            while (socket?.Connected == true)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
        }

        private void OnInitThread()
        {
            try
            {
                listenThread = new Thread(() =>
                {
                    if (configuration.ListenerDeactivated || !working)
                        return;
                    try
                    {
                        socket = OnGetSocket(configuration.Connection.ListenPort);
                        socket.Bind(new IPEndPoint(IPAddress.Any, configuration.Connection.ListenPort));
                        
                        while (working)
                        {
                            EndPoint clientEp = new IPEndPoint(IPAddress.Any, 0);
                            var bytes = new byte[configuration.Connection.PackageSize];
                            int bytesReceived = socket.ReceiveFrom(bytes, ref clientEp);
                            if (bytesReceived > 0)
                            {
#if DISCOVER
                                _logger.Trace($"Received UDP DGRAM From:{clientEp} Bytes Count:{bytesReceived}");
#endif
                                var deviceGreeting = _discoveryPipeline.Incoming<DeviceGreeting>(bytes.Take(bytesReceived).ToArray());
                                if (deviceGreeting != null)
                                {
                                    _deviceManager.Change(deviceGreeting, (IPEndPoint)clientEp);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        // if we reach this point we need to restart
                        if (working)
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

        private Socket OnGetSocket(int port)
        {
            var endpoint = new IPEndPoint(IPAddress.Broadcast, port);
            return new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        }
    }
}
