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
        private readonly ISerializer _serializer;

        private System.Threading.Thread listenThread;
        private readonly string threadName = $"Thread_{nameof(DiscoverySender)}";

        private Setting setting;
        private bool working = false;

        public DiscoverySender(IProtocolLogger protocolLogger, ISerializer serializer)
        {
            _logger = protocolLogger;
            _serializer = serializer;
            OnInitThread();
        }

        public void Initialize(Setting setting)
        {
            this.setting = setting;
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
                        var socket = OnGetSocket(setting.SendPortDiscovery);
                        socket.EnableBroadcast = true;
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                        while (working)
                        {
                            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
                            socket.SendTo(_serializer.Serialize(OnGetDeviceGreeting()), new IPEndPoint(IPAddress.Broadcast, setting.ListenPortDiscovery));
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
            return new DeviceGreeting
            {
                Identifier = Environment.MachineName,
                FriendlyName = Environment.MachineName,
                Metadata = new DeviceMetadata
                {
                    Idiom = DeviceMetadata.IdiomType.Desktop,
                    Is64Bit = Environment.Is64BitOperatingSystem,
                    Machine = Environment.MachineName,
                    OS = Environment.OSVersion.ToString(),
                    PhysicalMemory = Environment.WorkingSet,
                    Processor = Environment.ProcessorCount,
                    User = Environment.UserName
                },
                Communication = new DeviceCommunication
                {
                    Data = new DeviceCommunicationSetting
                    {
                        ByteSize = setting.ListenPackageBytesData,
                        Port = setting.ListenPortData
                    },
                    LowData = new DeviceCommunicationSetting
                    {
                        ByteSize = setting.ListenPackageBytesLowData,
                        Port = setting.ListenPortLowData
                    }
                }
            };
        }
    }
}
