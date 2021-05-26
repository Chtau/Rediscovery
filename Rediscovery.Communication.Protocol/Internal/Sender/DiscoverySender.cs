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
        private System.Threading.Thread listenThread;
        private readonly string threadName = $"Thread_{nameof(DiscoverySender)}";

        private Setting setting;
        private bool working = false;

        public DiscoverySender(IProtocolLogger protocolLogger = null)
        {
            _logger = protocolLogger ?? new ProtocolLogger();
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
                        while (working)
                        {
                            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
                            var data = System.Text.Encoding.ASCII.GetBytes("Hello");
                            socket.Send(data, data.Length, SocketFlags.Broadcast);
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
            //return Network.CreateSocket(port, SocketType.Dgram, ProtocolType.Udp);
            var endpoint = new IPEndPoint(IPAddress.Broadcast, port);
            return new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        }
    }
}
