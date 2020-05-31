using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationBase
{
    public class ConnectionProviderSignalR : IConnectionProvider<HubConnection>
    {
        private string baseUrl;
        private string token;
        private string _hubLink;
        private Protocol _protocol;
        private SharedBase.Logging.ILogger _logger;

        private HubConnection connection;

        public event EventHandler<(ConnectionConfiguration Config, bool IsConnected)> ConnectionChanged;
        public event EventHandler ConnectionClosed;

        public bool IsConnected
        {
            get
            {
                if (connection != null)
                    return connection.State == HubConnectionState.Connected;
                return false;
            }
        }

        public HubConnection CurrentConnection
        {
            get
            {
                return connection;
            }
        }

        public string BaseUrl => baseUrl;

        public string Token => token;

        private async Task<HubConnection> OnGetConnection(ConnectionConfiguration model, bool shouldUseToken = true)
        {
            if (model == null)
                return null;
            try
            {
                token = model.Token;
                if (connection != null)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (connection.State == HubConnectionState.Connected)
                            break;
                        await Task.Delay(50);
                    }
                    if (connection.State == HubConnectionState.Connected)
                    {
                        return connection;
                    }
                    else
                    {
                        _logger.LogTrace($"Reconnect to connection {model.DisplayName} ({DateTime.Now.ToString()})");
                        await connection.StopAsync();
                        await connection.DisposeAsync();
                        connection = null;
                        //return await OnGetConnection(model, shouldUseToken);
                    }
                }

                baseUrl = _protocol.ToProtocolValue() + model.Address;
                string url = baseUrl + _hubLink;
                _logger.LogTrace($"Try do connect to {model.DisplayName} with Address:{url} ({DateTime.Now.ToString()})");

                var handler = new HttpClientHandler
                {
                    //CheckCertificateRevocationList = false
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
                    //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                };

                if (shouldUseToken)
                {
                    connection = new HubConnectionBuilder()
                    .WithUrl(url, Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(model.Token);
                        options.HttpMessageHandlerFactory = _ => handler;
                        options.WebSocketConfiguration = sockets =>
                        {
                            sockets.RemoteCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
                        };
                    })
                    .ConfigureLogging(logging =>
                    {
                        // Log to the Output Window
                        //logging.AddDebug();

                        // This will set ALL logging to Debug level
                        //logging.SetMinimumLevel(LogLevel.Debug);
                    })
                    .Build();
                }
                else
                {
                    // TODO: https://docs.microsoft.com/en-us/aspnet/core/signalr/security?view=aspnetcore-3.1
                    // TODO: https://devblogs.microsoft.com/aspnet/configuring-https-in-asp-net-core-across-different-platforms/
                    // TODO: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.dangerousacceptanyservercertificatevalidator?view=netcore-3.0
                    // TODO: https://damienbod.com/2019/09/07/using-certificate-authentication-with-ihttpclientfactory-and-httpclient/
                    // TODO: https://github.com/dotnet/aspnetcore/issues/16919
                    // TODO: https://github.com/dotnet/aspnetcore/issues/14102
                    // TODO: https://github.com/dotnet/aspnetcore/issues/11408

                    connection = new HubConnectionBuilder()
                    .WithUrl(url, Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets, options =>
                    {
                        options.HttpMessageHandlerFactory = _ => handler;
                        options.WebSocketConfiguration = sockets =>
                        {
                            sockets.RemoteCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
                        };
                    })
                    .ConfigureLogging(logging =>
                    {
                        // Log to the Output Window
                        //logging.AddDebug();

                        // This will set ALL logging to Debug level
                        //logging.SetMinimumLevel(LogLevel.Debug);
                    })
                    .Build();
                }
                await connection.StartAsync();
                for (int i = 0; i < 50; i++)
                {
                    await Task.Delay(50);
                    if (connection.State == HubConnectionState.Connected)
                        break;
                }
                ConnectionChanged?.Invoke(this, (model, IsConnected));
                if (connection != null)
                {
                    connection.Closed += (Exception arg) =>
                    {
                        ConnectionChanged?.Invoke(this, (model, IsConnected));
                        ConnectionClosed?.Invoke(this, EventArgs.Empty);
                        return null;
                    };
                    connection.Reconnected += (string arg) =>
                    {
                        ConnectionChanged?.Invoke(this, (model, IsConnected));
                        return null;
                    };
                }
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get a valid HubConnection");
                return null;
            }
        }

        public void Init(SharedBase.Logging.ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP)
        {
            _hubLink = hubLink;
            _protocol = protocol;
            _logger = logger;
        }

        public async Task<(HubConnection Connection, bool Result)> Connect(Action<bool, HubConnection> connectCallback, ConnectionConfiguration model, bool shouldUseToken = true)
        {
            var con = await OnGetConnection(model, shouldUseToken);
            bool connected = IsConnected;
            connectCallback?.Invoke(connected, con);
            return (con, connected);
        }

        public async Task CloseConnection()
        {
            if (connection != null)
            {
                await connection.StopAsync();
                await connection.DisposeAsync();
                connection = null;
            }
            ConnectionClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
