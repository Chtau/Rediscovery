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
    // TODO: to support Balzor/WASM we need a communication implemenation based on Websocket or HttpClient
    // TODO: https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_client_applications

    internal class WebsocketCommunication : ICommunication
    {
        private readonly IProtocolLogger _logger;
        private readonly IDeviceManager _deviceManager;
        private readonly IDiagnosticPackage _diagnosticPackage;
        private readonly IEncryption _encryption;
        private readonly Dictionary<string, System.Net.WebSockets.ClientWebSocket> _sender = new Dictionary<string, System.Net.WebSockets.ClientWebSocket>();

        private Task listenTask;
        private CancellationTokenSource listenCancelationTokenSource;
        private bool listenerWorking = false;
        private System.Net.WebSockets.ClientWebSocket listener;

        private ConnectionListenConfiguration configuration;

        public event EventHandler<byte[]> Receive;

        public WebsocketCommunication(IProtocolLogger logger,
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
                        await socket.SendAsync(payload.Payload, System.Net.WebSockets.WebSocketMessageType.Binary, true, listenCancelationTokenSource.Token);
                        _diagnosticPackage.BytesSend(bytesToSend);
                    }
                    else
                    {
                        throw new NotSupportedException($"Type:\"{communicationPayload.GetType().FullName}\" is not supported in \"{nameof(WebsocketCommunication)}\"");
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
            OnStartListener();
        }

        public void Stop()
        {
            try
            {
                listenerWorking = false;
                if (listener != null)
                {
                    try
                    {
                        listener.Abort();
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
            while (listener?.State != System.Net.WebSockets.WebSocketState.Closed)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
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
                listenTask = new Task(async () =>
                {
                    await OnListenToSocket();
                }, listenCancelationTokenSource.Token, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async Task OnListenToSocket()
        {
            if (configuration.Disable)
                return;
            if (!listenerWorking)
                return;
            try
            {
                listener = new System.Net.WebSockets.ClientWebSocket();
                Uri uri = new Uri("ws://localhost:49889/");
                await listener.ConnectAsync(uri, listenCancelationTokenSource.Token);
                var byteBuffer = new byte[configuration.PackageSize + _encryption.SymmetricEncryptionSignatureLength];

                while (listenerWorking)
                {
                    var result = await listener.ReceiveAsync(byteBuffer, listenCancelationTokenSource.Token);
                    Receive?.Invoke(this, byteBuffer);
                    _diagnosticPackage.BytesReceived(result.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                if (listenerWorking)
                    OnStartListener();
            }
        }

        private async Task<System.Net.WebSockets.ClientWebSocket> OnGetSocket(string identifier, string ip, int port)
        {
            identifier = identifier.ToLower();
            System.Net.WebSockets.ClientWebSocket sender;
            if (_sender.ContainsKey(identifier))
            {
                sender = _sender[identifier];
                if (sender.State != System.Net.WebSockets.WebSocketState.Closed)
                    return sender;
                _sender.Remove(identifier);
            }
            Uri uri = new Uri($"ws://{ip}:{port}/");
            sender = new System.Net.WebSockets.ClientWebSocket();
            await sender.ConnectAsync(uri, listenCancelationTokenSource.Token);
            _sender.Add(identifier, sender);
            return sender;
        }
    }
}
