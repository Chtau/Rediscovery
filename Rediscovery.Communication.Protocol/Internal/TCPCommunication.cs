using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class TCPCommunication : ICommunication
    {
        private readonly IProtocolLogger _logger;
        private readonly IDeviceManager _deviceManager;
        private readonly Dictionary<string, Socket> _sender = new Dictionary<string, Socket>();
        private readonly string _listenerThreadName = $"Thread_Listener_{nameof(TCPCommunication)}";

        private Thread listenThread;
        private bool listenerWorking = false;

        private BaseConfiguration configuration;

        public event EventHandler<CommunicationPayload> Receive;

        public TCPCommunication(IProtocolLogger logger,
            IDeviceManager deviceManager)
        {
            _logger = logger;
            _deviceManager = deviceManager;
            OnInitListenerThread();
        }

        public void Initialize(BaseConfiguration config)
        {
            configuration = config;
        }

        public bool Send(CommunicationPayload communicationPayload)
        {
            try
            {
                var greeting = _deviceManager.GetGreeting(communicationPayload.ReceiverIdentifier);
                if (greeting == null)
                    return false;
                var socket = OnGetSocket(greeting);
                int bytesToSend = communicationPayload.Payload.Length;
                int sendBytes = socket.Send(communicationPayload.Payload,
                    0,
                    bytesToSend,
                    SocketFlags.None);
                return bytesToSend == sendBytes;
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        public void Start()
        {
            OnStartListener();
        }

        public void Stop()
        {
            try
            {
                listenerWorking = false;
                listenThread?.Abort();
            }
            catch (PlatformNotSupportedException) { }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private Socket OnGetSocket(DeviceGreetingReceived deviceGreeting)
        {
            Socket sender;
            if (_sender.ContainsKey(deviceGreeting.Device.Identifier))
            {
                sender = _sender[deviceGreeting.Device.Identifier];
                if (sender.Connected)
                    return sender;
                _sender.Remove(deviceGreeting.Device.Identifier);
            }
            var endpoint = new IPEndPoint(IPAddress.Parse(deviceGreeting.IP), deviceGreeting.Device.Communication.Data.Port);
            sender = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(endpoint);
            _sender.Add(deviceGreeting.Device.Identifier, sender);
            return sender;
        }

        private void OnStartListener()
        {
            try
            {
                listenerWorking = true;
                listenThread.Start();
            }
            catch (System.Threading.ThreadStateException tsEx)
            {
                _logger.Warning(tsEx);
                OnInitListenerThread();
                try
                {
                    listenerWorking = true;
                    listenThread.Start();
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
        }

        private void OnInitListenerThread()
        {
            try
            {
                listenThread = new System.Threading.Thread(() =>
                {
                    OnListenToSocket();
                })
                {
                    Name = $"{_listenerThreadName}_{DateTime.Today.Ticks}"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnListenToSocket()
        {
            try
            {
                Socket listener = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(IPAddress.Any, configuration.Connection.ListenPort));
                listener.Listen(10);

                var byteBuffer = new byte[configuration.Connection.PackageSize];

                while (listenerWorking)
                {
                    Socket handler = listener.Accept();
                    //data = null;

                    // An incoming connection needs to be processed.  
                    while (listenerWorking)
                    {
                        int bytesRec = handler.Receive(byteBuffer);
                        Receive?.Invoke(this, new CommunicationPayload(byteBuffer, null));
                        // TODO: add data to the pipeline incoming
                        /*data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }*/
                    }

                    // Show the data on the console.  
                    //Console.WriteLine("Text received : {0}", data);

                    // Echo the data back to the client.  
                    //byte[] msg = Encoding.ASCII.GetBytes(data);

                    //handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                OnStartListener();
            }
        }
    }
}
