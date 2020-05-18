using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
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
        private ILogger _logger;

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
                        _logger.Message($"Reconnect to connection {model.DisplayName} ({DateTime.Now.ToString()})");
                        await connection.StopAsync();
                        await connection.DisposeAsync();
                        connection = null;
                        //return await OnGetConnection(model, shouldUseToken);
                    }
                }

                baseUrl = _protocol.ToProtocolValue() + model.Address;
                string url = baseUrl + _hubLink;
                _logger.Message($"Try do connect to {model.DisplayName} with Address:{url} ({DateTime.Now.ToString()})");
                if (shouldUseToken)
                {
                    connection = new HubConnectionBuilder()
                    .WithUrl(url, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(model.Token);
                    })
                    .Build();
                }
                else
                {
                    connection = new HubConnectionBuilder()
                    .WithUrl(url)
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
                return connection;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return null;
            }
        }

        public void Init(ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP)
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
