using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Internal.Diagnostic;
using Rediscovery.Communication.Protocol.Internal.Encryption;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    // TODO: https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_server

    internal class WebSocketServerCommunication : ICommunication
    {
        private readonly IProtocolLogger _logger;
        private readonly IDeviceManager _deviceManager;
        private readonly IDiagnosticPackage _diagnosticPackage;
        private readonly IEncryption _encryption;
        private readonly Dictionary<string, Socket> _sender = new Dictionary<string, Socket>();

        private Task listenTask;
        private CancellationTokenSource listenCancelationTokenSource;
        private bool listenerWorking = false;
        private Socket handler;
        private Socket listener;

        private ConnectionListenConfiguration configuration;

        public event EventHandler<byte[]> Receive;

        public WebSocketServerCommunication(IProtocolLogger logger,
            IDeviceManager deviceManager,
            IDiagnosticPackage diagnosticPackage,
            IEncryption encryption)
        {
            _encryption = encryption;
            _logger = logger;
            _diagnosticPackage = diagnosticPackage;
            _deviceManager = deviceManager;

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
                if (communicationPayload is PortCommunicationPayload payload)
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
                }
                else
                {
                    throw new NotSupportedException($"Type:\"{communicationPayload.GetType().FullName}\" is not supported in \"{nameof(TCPCommunication)}\"");
                }
            }
            catch (Exception ex)
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
                listenCancelationTokenSource?.Cancel();
                listenCancelationTokenSource = null;
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
                listenTask.Start();
            }
            catch (ThreadStateException tsEx)
            {
                // TODO: improve restart logic with delay to reduce spam
                _logger.Warning($"Task ID:\"{listenTask.Id}\" State:{listenTask.Status}");
                _logger.Warning(tsEx);
                if (listenTask.Status == TaskStatus.Running)
                {
                    try
                    {
                        listenCancelationTokenSource.Cancel();
                        listenCancelationTokenSource = null;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
                OnInitListenerThread();
                try
                {
                    listenerWorking = true;
                    listenTask.Start();
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
                listenCancelationTokenSource = new CancellationTokenSource();
                listenTask = new Task(() =>
                {
                    OnListenToSocket();
                }, listenCancelationTokenSource.Token, TaskCreationOptions.LongRunning);
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

                var byteBuffer = new byte[configuration.PackageSize + _encryption.SymmetricEncryptionSignatureLength];

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

        internal void OnOpenWebSocket()
        {
            try
            {
                string ip = "127.0.0.1";
                int port = 49889;
                var server = new TcpListener(IPAddress.Parse(ip), port);

                server.Start();
                _logger.Trace($"Server has started on {ip}:{port}, Waiting for a connection...");

                TcpClient client = server.AcceptTcpClient();
                _logger.Trace("A client connected.");

                NetworkStream stream = client.GetStream();

                // enter to an infinite cycle to be able to handle every change in stream
                while (true)
                {
                    while (!stream.DataAvailable) ;
                    while (client.Available < 3) ; // match against "get"

                    byte[] bytes = new byte[client.Available];
                    stream.Read(bytes, 0, client.Available);
                    string s = Encoding.UTF8.GetString(bytes);

                    if (Regex.IsMatch(s, "^GET", RegexOptions.IgnoreCase))
                    {
                        _logger.Trace($"=====Handshaking from client=====\n{s}");

                        // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                        // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                        // 3. Compute SHA-1 and Base64 hash of the new value
                        // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                        string swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                        string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                        byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                        string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                        // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                        byte[] response = Encoding.UTF8.GetBytes(
                            "HTTP/1.1 101 Switching Protocols\r\n" +
                            "Connection: Upgrade\r\n" +
                            "Upgrade: websocket\r\n" +
                            "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                        stream.Write(response, 0, response.Length);
                    }
                    else
                    {
                        bool fin = (bytes[0] & 0b10000000) != 0,
                            mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"

                        int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                            msglen = bytes[1] - 128, // & 0111 1111
                            offset = 2;

                        if (msglen == 126)
                        {
                            // was ToUInt16(bytes, offset) but the result is incorrect
                            msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                            offset = 4;
                        }
                        else if (msglen == 127)
                        {
                            _logger.Trace("TODO: msglen == 127, needs qword to store msglen");
                            // i don't really know the byte order, please edit this
                            // msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
                            // offset = 10;
                        }

                        if (msglen == 0)
                            _logger.Trace("msglen == 0");
                        else if (mask)
                        {
                            byte[] decoded = new byte[msglen];
                            byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                            offset += 4;

                            for (int i = 0; i < msglen; ++i)
                                decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

                            string text = Encoding.UTF8.GetString(decoded);
                            _logger.Trace(text);
                        }
                        else
                            _logger.Trace("mask bit not set");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
