using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Internal.Diagnostic;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    internal class TCPCommunication : ICommunication
    {
        private readonly IProtocolLogger _logger;
        private readonly IDeviceManager _deviceManager;
        private readonly IDiagnosticPackage _diagnosticPackage;
        private readonly Dictionary<string, Socket> _sender = new Dictionary<string, Socket>();
        private readonly string _listenerThreadName = $"Thread_Listener_{nameof(TCPCommunication)}";
        private readonly bool _isLarge = false;

        private Thread listenThread;
        private bool listenerWorking = false;
        private Socket handler;
        private Socket listener;

        private ConnectionConfiguration configuration;

        public event EventHandler<byte[]> Receive;

        public TCPCommunication(IProtocolLogger logger,
            IDeviceManager deviceManager,
            IDiagnosticPackage diagnosticPackage, bool isLarge = false)
        {
            _isLarge = isLarge;
            _logger = logger;
            _diagnosticPackage = diagnosticPackage;
            _deviceManager = deviceManager;
            OnInitListenerThread();
        }

        public void Initialize(ConnectionConfiguration config)
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
                _diagnosticPackage.BytesSend(sendBytes);
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
                if (handler != null)
                {
                    try
                    {
                        handler.Close();
                        handler.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
                if (listener != null)
                {
                    try
                    {
                        listener.Close();
                        listener.Dispose();
                    }
                    catch (Exception ex)
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
            while (listener?.Connected == true || handler?.Connected == true)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
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
            int port = deviceGreeting.Device.Communication.Data.Port;
            if (_isLarge)
                port = deviceGreeting.Device.Communication.DataLarge.Port;
            var endpoint = new IPEndPoint(IPAddress.Parse(deviceGreeting.IP), port);
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
            catch (ThreadStateException tsEx)
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
                listenThread = new Thread(() =>
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
            if (!listenerWorking)
                return;
            try
            {
                listener = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(IPAddress.Any, configuration.ListenPort));
                listener.Listen(10);

                var byteBuffer = new byte[configuration.PackageSize];

                while (listenerWorking)
                {
                    handler = listener.Accept();

                    // An incoming connection needs to be processed.  
                    while (listenerWorking)
                    {
                        int bytesRec = handler.Receive(byteBuffer);
                        Receive?.Invoke(this, byteBuffer);
                        _diagnosticPackage.BytesReceived(bytesRec);
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                if (listenerWorking)
                    OnStartListener();
            }
        }
    }
}
