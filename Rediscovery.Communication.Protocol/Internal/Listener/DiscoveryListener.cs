using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class DiscoveryListener
    {
        private readonly IProtocolLogger _logger;
        private readonly IPackagePipeline _packagePipeline;

        private System.Threading.Thread listenThread;
        private readonly string threadName = $"Thread_{nameof(DiscoveryListener)}";
        private List<DeviceGreetingReceived> _devices = new List<DeviceGreetingReceived>();

        private DiscoveryConfiguration configuration;
        private bool working = false;

        public List<DeviceGreeting> Devices => _devices.Select(x => x.Device).ToList();

        public DiscoveryListener(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline)
        {
            _logger = protocolLogger;
            _packagePipeline = packagePipeline;
            OnInitThread();
        }

        public void Initialize(DiscoveryConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Start()
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

        public bool Stop()
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

        private void OnInitThread()
        {
            try
            {
                listenThread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        var socket = OnGetSocket(configuration.Connection.ListenPort);
                        socket.Bind(new IPEndPoint(IPAddress.Any, configuration.Connection.ListenPort));
                        //socket.EnableBroadcast = true;
                        while (working)
                        {
                            EndPoint clientEp = new IPEndPoint(IPAddress.Any, 0);
                            var bytes = new byte[512];
                            int bytesReceived = socket.ReceiveFrom(bytes, ref clientEp);
                            if (bytesReceived > 0)
                            {
                                System.Diagnostics.Trace.TraceInformation($"Received UDP DGRAM Bytes Count:{bytesReceived}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        // if we reach this point we need to restart
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
