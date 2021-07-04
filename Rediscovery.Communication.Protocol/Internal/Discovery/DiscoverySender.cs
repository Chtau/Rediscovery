using MessagePack;
using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Discovery
{
    internal class DiscoverySender
    {
        private readonly IProtocolLogger _logger;
        private readonly IDiscoveryPipeline _discoveryPipeline;
        private readonly IDeviceManager _deviceManager;

        private Task listenTask;
        private CancellationTokenSource listenCancellationTokenSource;
        private DiscoveryConfiguration configuration;
        private ConnectionListenConfiguration connectionListenConfigurationData;
        private ConnectionListenConfiguration connectionListenConfigurationLarge;
        private ConnectionListenConfiguration connectionListenConfigurationHandshake;
        private bool working = false;
        private string currentIdentifier;
        private string currentFriendlyName;
        private DeviceMetadata.IdiomType currentIdiom = DeviceMetadata.IdiomType.Desktop;
        private DeviceGreeting greeting;
        private TimeSpan discoverySendTimeout = TimeSpan.FromMilliseconds(100);

        public DiscoverySender(IProtocolLogger protocolLogger, 
            IDiscoveryPipeline discoveryPipeline,
            IDeviceManager deviceManager)
        {
            _logger = protocolLogger;
            _discoveryPipeline = discoveryPipeline;
            _deviceManager = deviceManager;
            OnInitThread();
        }

        public void Initialize(DiscoveryConfiguration configuration, ConnectionListenConfiguration listenConfigurationHandshake, ConnectionListenConfiguration listenConfigurationData, ConnectionListenConfiguration listenConfigurationLarge)
        {
            this.configuration = configuration;
            connectionListenConfigurationHandshake = listenConfigurationHandshake;
            connectionListenConfigurationData = listenConfigurationData;
            connectionListenConfigurationLarge = listenConfigurationLarge;
        }

        public bool Start()
        {
            try
            {
                if (configuration.SenderDeactivated)
                    return true;
                working = true;
                listenTask.Start();
                return true;
            }
            catch (System.Threading.ThreadStateException tsEx)
            {
                _logger.Warning(tsEx);
                OnInitThread();
                try
                {
                    working = true;
                    listenTask.Start();
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
                listenCancellationTokenSource?.Cancel();
                listenCancellationTokenSource = null;
            }
            catch (PlatformNotSupportedException) { }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void SetIdentifier(string identifier) => currentIdentifier = identifier;
        public void SetFriendlyName(string friendlyName) => currentFriendlyName = friendlyName;
        public void SetIdiom(DeviceMetadata.IdiomType idiomType) => currentIdiom = idiomType;

        private void OnInitThread()
        {
            try
            {
                listenCancellationTokenSource = new CancellationTokenSource();
                listenTask = new Task(() =>
                {
                    if (configuration.SenderDeactivated)
                        return;
                    try
                    {
                        Parallel.ForEach(configuration.Connection.SendPort, (port) =>
                        {
                            var socket = OnGetSocket(port);
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
                                    var device = OnGetDeviceGreeting();
                                    var deviceRaw = _discoveryPipeline.Outgoing(device);
#if DISCOVER
                                _logger.Trace($"Broadcast Greeting for Peer:{device.Identifier} Hops:{device.Hops} Bytes:{deviceRaw.Length}");
#endif
                                    socket.SendTo(deviceRaw, new IPEndPoint(IPAddress.Broadcast, port));
                                    var deviceGreetings = _deviceManager.Devices;
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
                                                        PackageSize = connectionListenConfigurationData.PackageSize,
                                                        Port = connectionListenConfigurationData.Port,
                                                    },
                                                    Large = new DeviceCommunicationSetting
                                                    {
                                                        PackageSize = connectionListenConfigurationLarge.PackageSize,
                                                        Port = connectionListenConfigurationLarge.Port
                                                    },
                                                    Handshake = new DeviceCommunicationSetting
                                                    {
                                                        PackageSize = connectionListenConfigurationHandshake.PackageSize,
                                                        Port = connectionListenConfigurationHandshake.Port
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
                                            var raw = _discoveryPipeline.Outgoing(peerGreeting);
#if DISCOVER
                                        _logger.Trace($"Broadcast Greeting for Peer:{deviceGreeting.Identifier} Hops:{deviceGreeting.Hops} Bytes:{raw.Length}");
#endif
                                            socket.SendTo(raw, new IPEndPoint(IPAddress.Broadcast, port));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(ex);
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        // if we reach this point we need to restart
                        Start();
                    }
                }, listenCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
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
            if (greeting.Communication.Handshake == null
                || greeting.Communication.Handshake.Port == -1)
            {
                greeting.Communication.Handshake = new DeviceCommunicationSetting
                {
                    PackageSize = connectionListenConfigurationHandshake.PackageSize,
                    Port = connectionListenConfigurationHandshake.Port
                };
            }
            if (greeting.Communication.Data == null
                || greeting.Communication.Data.Port == -1)
            {
                greeting.Communication.Data = new DeviceCommunicationSetting
                {
                    PackageSize = connectionListenConfigurationData.PackageSize,
                    Port = connectionListenConfigurationData.Port
                };
            }
            if (greeting.Communication.Large == null
                || greeting.Communication.Large.Port == -1)
            {
                greeting.Communication.Large = new DeviceCommunicationSetting
                {
                    PackageSize = connectionListenConfigurationLarge.PackageSize,
                    Port = connectionListenConfigurationLarge.Port
                };
            }
            return greeting;
        }
    }
}
