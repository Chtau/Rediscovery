using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Internal.Diagnostic;
using Rediscovery.Communication.Protocol.Internal.Encryption;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    internal class WebSocketCommunication : ICommunication
    {
        struct Socket
        {
            public System.Net.WebSockets.ClientWebSocket WebSocket { get; }
            public CancellationTokenSource CancellationToken { get; }

            public Socket(System.Net.WebSockets.ClientWebSocket webSocket, CancellationTokenSource cancellationToken)
            {
                WebSocket = webSocket;
                CancellationToken = cancellationToken;
            }
        }

        private readonly IProtocolLogger _logger;
        private readonly IDeviceManager _deviceManager;
        private readonly IDiagnosticPackage _diagnosticPackage;
        private readonly IEncryption _encryption;
        private readonly Dictionary<string, Socket> _sockets = new Dictionary<string, Socket>();

        private ConnectionListenConfiguration configuration;

        public event EventHandler<byte[]> Receive;

        public WebSocketCommunication(IProtocolLogger logger,
            IDeviceManager deviceManager,
            IDiagnosticPackage diagnosticPackage,
            IEncryption encryption)
        {
            _encryption = encryption;
            _logger = logger;
            _diagnosticPackage = diagnosticPackage;
            _deviceManager = deviceManager;
        }

        public void Initialize(ConnectionListenConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Send<TPayload>(TPayload communicationPayload) where TPayload : CommunicationPayload
        {
            Task.Run(async () =>
            {
                try
                {
                    if (communicationPayload is PortCommunicationPayload payload)
                    {
                        var ip = _deviceManager.GetIP(payload.ReceiverIdentifier);
                        if (string.IsNullOrWhiteSpace(ip))
                            return;
                        var socket = await OnGetSocket(payload.ReceiverIdentifier, ip, payload.Port);
                        int bytesToSend = payload.Payload.Length;
                        await socket.WebSocket.SendAsync(payload.Payload, System.Net.WebSockets.WebSocketMessageType.Binary, true, socket.CancellationToken.Token);
                        _diagnosticPackage.BytesSend(bytesToSend);
                    }
                    else
                    {
                        throw new NotSupportedException($"Type:\"{communicationPayload.GetType().FullName}\" is not supported in \"{nameof(WebSocketCommunication)}\"");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            });
            return true;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        private async Task<Socket> OnGetSocket(string identifier, string ip, int port)
        {
            identifier = identifier.ToLower();
            Socket socket;
            if (_sockets.ContainsKey(identifier))
            {
                socket = _sockets[identifier];
                if (socket.WebSocket.State != System.Net.WebSockets.WebSocketState.Closed)
                    return socket;
                _sockets.Remove(identifier);
            }
            Uri uri = new Uri($"ws://{ip}:{port}/");
            var tokenSource = new CancellationTokenSource();
            socket = new Socket(new System.Net.WebSockets.ClientWebSocket(), tokenSource);
            await socket.WebSocket.ConnectAsync(uri, tokenSource.Token);
            _sockets.Add(identifier, socket);
            _ = Task.Run(async () =>
            {
                do
                {
                    var byteBuffer = new byte[configuration.PackageSize + _encryption.SymmetricEncryptionSignatureLength];
                    var received = await socket.WebSocket.ReceiveAsync(byteBuffer, tokenSource.Token);
                    // we cloud create a byte buffer aslong as received is not EndOfMessage
                    Receive?.Invoke(this, byteBuffer);
                    _diagnosticPackage.BytesReceived(received.Count);
                } while (!tokenSource.Token.IsCancellationRequested);
            });
            return socket;
        }
    }
}
