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
        private readonly int _packageEncryptionSignatureLength;

        private Thread listenThread;
        private bool listenerWorking = false;
        private Socket handler;
        private Socket listener;

        private ConnectionListenConfiguration configuration;

        public event EventHandler<byte[]> Receive;

        public TCPCommunication(IProtocolLogger logger,
            IDeviceManager deviceManager,
            IDiagnosticPackage diagnosticPackage, string threadName = null,
            int packageEncryptionSignatureLength = 0)
        {
            _packageEncryptionSignatureLength = packageEncryptionSignatureLength;
            _logger = logger;
            _diagnosticPackage = diagnosticPackage;
            _deviceManager = deviceManager;
            if (!string.IsNullOrWhiteSpace(threadName))
                _listenerThreadName += $"_{threadName}";

            OnInitListenerThread();
        }

        public void Initialize(ConnectionListenConfiguration config)
        {
            configuration = config;
        }

        public bool Send<TPayload>(TPayload communicationPayload) where TPayload : CommunicationPayload
        {
            try
            {
                if (communicationPayload is TCPCommunicationPayload payload)
                {
                    var ip = _deviceManager.GetIP(payload.ReceiverIdentifier);
                    if (string.IsNullOrWhiteSpace(ip))
                        return false;
                    var socket = OnGetSocket(payload.ReceiverIdentifier, ip, payload.Port);
                    int bytesToSend = payload.Payload.Length;
                    int sendBytes = socket.Send(payload.Payload,
                        0,
                        bytesToSend,
                        SocketFlags.None);
                    _diagnosticPackage.BytesSend(sendBytes);
                    return bytesToSend == sendBytes;
                } else
                {
                    throw new NotSupportedException($"Type:\"{communicationPayload.GetType().FullName}\" is not supported in \"{nameof(TCPCommunication)}\"");
                }
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

        private Socket OnGetSocket(string identifier, string ip, int port)
        {
            identifier = identifier.ToLower();
            Socket sender;
            if (_sender.ContainsKey(identifier))
            {
                sender = _sender[identifier];
                if (sender.Connected)
                    return sender;
                _sender.Remove(identifier);
            }
            var endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            sender = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(endpoint);
            _sender.Add(identifier, sender);
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
                // TODO: improve restart logic with delay to reduce spam
                _logger.Warning($"Thread Name:\"{listenThread.Name}\" State:{listenThread.ThreadState}");
                _logger.Warning(tsEx);
                if (listenThread.ThreadState == ThreadState.Running)
                {
                    try
                    {
                        listenThread.Abort();
                    } catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
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
            if (configuration.Disable)
                return;
            if (!listenerWorking)
                return;
            try
            {
                listener = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(IPAddress.Any, configuration.Port));
                listener.Listen(10);

                var byteBuffer = new byte[configuration.PackageSize + _packageEncryptionSignatureLength];

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
