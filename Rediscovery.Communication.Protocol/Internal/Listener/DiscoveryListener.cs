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
        private TimeSpan deviceTimeoutOffset = TimeSpan.FromSeconds(10);
        private string currentIdentifier;

        public List<DeviceGreeting> Devices => _devices.Select(x => x.Device).ToList();
        public event EventHandler<string> DevicesChanged;

        public DiscoveryListener(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline)
        {
            _logger = protocolLogger;
            _packagePipeline = packagePipeline;
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
                listenThread?.Abort();
            }
            catch (PlatformNotSupportedException) { }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public DeviceGreetingReceived GetDeviceGreeting(string identifier) => _devices.First(x => x.Device.Identifier == identifier);

        private void OnInitThread()
        {
            try
            {
                listenThread = new System.Threading.Thread(() =>
                {
                    if (configuration.ListenerDeactivated)
                        return;
                    try
                    {
                        var socket = OnGetSocket(configuration.Connection.ListenPort);
                        socket.Bind(new IPEndPoint(IPAddress.Any, configuration.Connection.ListenPort));
                        
                        while (working)
                        {
                            EndPoint clientEp = new IPEndPoint(IPAddress.Any, 0);
                            var bytes = new byte[configuration.Connection.PackageSize];
                            int bytesReceived = socket.ReceiveFrom(bytes, ref clientEp);
                            if (bytesReceived > 0)
                            {
                                _logger.Trace($"Received UDP DGRAM From:{clientEp} Bytes Count:{bytesReceived}");
                                var deviceGreeting = _packagePipeline.Incoming<DeviceGreeting>(bytes.Take(bytesReceived).ToArray());
                                if (deviceGreeting != null)
                                {
                                    OnHandleReceivedDevices(deviceGreeting, (IPEndPoint)clientEp);
                                }
                            }
                            OnHandleTimeoutDevices();
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

        private void OnHandleReceivedDevices(DeviceGreeting deviceGreeting, IPEndPoint ipEndPoint)
        {
            if (deviceGreeting.Identifier == currentIdentifier)
                return;
            try
            {
                var d = _devices.FirstOrDefault(x => x.Device.Identifier == deviceGreeting.Identifier);
                if (d != null)
                {
                    if (d.Update(deviceGreeting, ipEndPoint.Address.ToString()))
                    {
                        DevicesChanged?.Invoke(this, deviceGreeting.Identifier);
                    }
                }
                else
                {
                    _devices.Add(new DeviceGreetingReceived(deviceGreeting, ipEndPoint.Address.ToString()));
                    DevicesChanged?.Invoke(this, deviceGreeting.Identifier);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnHandleTimeoutDevices()
        {
            try
            {
                _devices.RemoveAll(x => x.Received < (DateTime.UtcNow - deviceTimeoutOffset));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
