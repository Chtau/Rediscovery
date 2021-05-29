using MessagePack;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class DiscoverySender
    {
        private readonly IProtocolLogger _logger;
        private readonly IPackagePipeline _packagePipeline;
        private readonly string threadName = $"Thread_{nameof(DiscoverySender)}";

        private System.Threading.Thread listenThread;
        private DiscoveryConfiguration configuration;
        private bool working = false;
        private string currentIdentifier;

        public DeviceGreeting Greeting { get; set; }

        public DiscoverySender(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline)
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

        public void SetIdentifier(string identifier) => currentIdentifier = identifier;

        private void OnInitThread()
        {
            try
            {
                listenThread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        var socket = OnGetSocket(configuration.Connection.SendPort);
                        socket.EnableBroadcast = true;
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                        while (working)
                        {
                            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
                            // TODO: send own device greeting and all other devices with +1 hop
                            socket.SendTo(_packagePipeline.Outgoing(OnGetDeviceGreeting()), new IPEndPoint(IPAddress.Broadcast, configuration.Connection.SendPort));
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

        private DeviceGreeting OnGetDeviceGreeting()
        {
            if (Greeting == null)
            {
                Greeting = new DeviceGreeting
                {
                    Identifier = currentIdentifier,
                    FriendlyName = Environment.MachineName
                };
            }
            if (Greeting.Metadata == null)
            {
                Greeting.Metadata = new DeviceMetadata
                {
                    Idiom = DeviceMetadata.IdiomType.Desktop,
                    Is64Bit = Environment.Is64BitOperatingSystem,
                    Machine = Environment.MachineName,
                    OS = Environment.OSVersion.ToString(),
                    PhysicalMemory = Environment.WorkingSet,
                    Processor = Environment.ProcessorCount,
                    User = Environment.UserName
                };
            }
            if (Greeting.Communication == null)
            {
                Greeting.Communication = new DeviceCommunication();
            }
            if (Greeting.Communication.Data == null)
            {
                Greeting.Communication.Data = new DeviceCommunicationSetting
                {
                    ByteSize = -1,//setting.ListenPackageBytesData,
                    Port = -1//setting.ListenPortData
                };
            }
            if (Greeting.Communication.LowData == null)
            {
                Greeting.Communication.LowData = new DeviceCommunicationSetting
                {
                    ByteSize = -1,//setting.ListenPackageBytesLowData,
                    Port = -1//setting.ListenPortLowData
                };
            }
            return Greeting;
        }
    }
}
