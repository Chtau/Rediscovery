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
        private ConnectionConfiguration connectionConfigurationData;
        private ConnectionConfiguration connectionConfigurationLowData;
        private bool working = false;
        private string currentIdentifier;
        private string currentFriendlyName;
        private DeviceMetadata.IdiomType currentIdiom = DeviceMetadata.IdiomType.Desktop;
        private DeviceGreeting greeting;
        private Func<List<DeviceGreeting>> deviceGreetingCallback;
        private TimeSpan discoverySendTimeout = TimeSpan.FromMilliseconds(100);

        public DiscoverySender(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline)
        {
            _logger = protocolLogger;
            _packagePipeline = packagePipeline;
            OnInitThread();
        }

        public void Initialize(DiscoveryConfiguration configuration, ConnectionConfiguration connectionConfigurationData, ConnectionConfiguration connectionConfigurationLowData)
        {
            this.configuration = configuration;
            this.connectionConfigurationData = connectionConfigurationData;
            this.connectionConfigurationLowData = connectionConfigurationLowData;
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
        public void SetFriendlyName(string friendlyName) => currentFriendlyName = friendlyName;
        public void SetIdiom(DeviceMetadata.IdiomType idiomType) => currentIdiom = idiomType;
        public void KnownDevicesCallback(Func<List<DeviceGreeting>> callback) => deviceGreetingCallback = callback;

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
                            System.Threading.Thread.Sleep(discoverySendTimeout);
                            // send own device greeting
                            // All other peers will be send with +1 hop
                            // and communication ports and package size will be used from this
                            // because the current device is a proxy for the peers
                            try
                            {
                                socket.SendTo(_packagePipeline.Outgoing(OnGetDeviceGreeting()), new IPEndPoint(IPAddress.Broadcast, configuration.Connection.SendPort));
                                var deviceGreetings = deviceGreetingCallback.Invoke();
                                if (deviceGreetings.Count > 0)
                                {
                                    foreach (var deviceGreeting in deviceGreetings)
                                    {
                                        var peerGreeting = new DeviceGreeting
                                        {
                                            Identifier = deviceGreeting.Identifier,
                                            FriendlyName = deviceGreeting.FriendlyName,
                                            Hops = deviceGreeting.Hops + 1,
                                            Communication = new DeviceCommunication
                                            {
                                                Data = new DeviceCommunicationSetting
                                                {
                                                    PackageSize = connectionConfigurationData.PackageSize,
                                                    Port = connectionConfigurationData.ListenPort
                                                },
                                                LowData = new DeviceCommunicationSetting
                                                {
                                                    PackageSize = connectionConfigurationLowData.PackageSize,
                                                    Port = connectionConfigurationLowData.ListenPort
                                                }
                                            },
                                            Metadata = new DeviceMetadata
                                            {
                                                Idiom = deviceGreeting.Metadata.Idiom,
                                                Is64Bit = deviceGreeting.Metadata.Is64Bit,
                                                Machine = deviceGreeting.Metadata.Machine,
                                                OS = deviceGreeting.Metadata.OS,
                                                PhysicalMemory = deviceGreeting.Metadata.PhysicalMemory,
                                                Processor = deviceGreeting.Metadata.Processor,
                                                User = deviceGreeting.Metadata.User
                                            }
                                        };
                                        socket.SendTo(_packagePipeline.Outgoing(peerGreeting), new IPEndPoint(IPAddress.Broadcast, configuration.Connection.SendPort));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex);
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

        private DeviceGreeting OnGetDeviceGreeting()
        {
            if (greeting == null)
            {
                greeting = new DeviceGreeting
                {
                    Identifier = currentIdentifier,
                    FriendlyName = currentFriendlyName ?? Environment.MachineName
                };
            }
            if (greeting.Metadata == null)
            {
                greeting.Metadata = new DeviceMetadata
                {
                    Idiom = currentIdiom,
                    Is64Bit = Environment.Is64BitOperatingSystem,
                    Machine = Environment.MachineName,
                    OS = Environment.OSVersion.ToString(),
                    PhysicalMemory = Environment.WorkingSet,
                    Processor = Environment.ProcessorCount,
                    User = Environment.UserName
                };
            }
            if (greeting.Communication == null)
            {
                greeting.Communication = new DeviceCommunication();
            }
            if (greeting.Communication.Data == null
                || greeting.Communication.Data.Port == -1)
            {
                greeting.Communication.Data = new DeviceCommunicationSetting
                {
                    PackageSize = connectionConfigurationData.PackageSize,
                    Port = connectionConfigurationData.ListenPort
                };
            }
            if (greeting.Communication.LowData == null
                || greeting.Communication.LowData.Port == -1)
            {
                greeting.Communication.LowData = new DeviceCommunicationSetting
                {
                    PackageSize = connectionConfigurationLowData.PackageSize,
                    Port = connectionConfigurationLowData.ListenPort
                };
            }
            return greeting;
        }
    }
}
