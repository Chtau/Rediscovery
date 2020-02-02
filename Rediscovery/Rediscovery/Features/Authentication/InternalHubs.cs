using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rediscovery.Features.Authentication
{
    public abstract class InternalHubs
    {
        const string Protocol = "http://";

        internal ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private readonly string _hubLink;

        private HubConnection connection;

        public event EventHandler<Models.Connection> ConnectionChanged;

        public bool IsConnected
        {
            get
            {
                if (connection != null)
                    return connection.State == HubConnectionState.Connected;
                return false;
            }
        }

        public InternalHubs(string hubLink)
        {
            _hubLink = hubLink;
        }

        internal async Task<HubConnection> OnGetConnection(Models.Connection model, bool shouldUseToken = true)
        {
            if (model == null)
                return null;
            try
            {
                if (connection != null)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (connection.State == HubConnectionState.Connected)
                            break;
                        await Task.Delay(50);
                    }
                    if (connection.State != HubConnectionState.Connected)
                    {
                        logger.Message($"Reconnect to connection {model.DisplayName} ({DateTime.Now.ToString()})");
                        await connection.StopAsync();
                        await connection.DisposeAsync();
                        connection = null;
                        return await OnGetConnection(model, shouldUseToken);
                    }
                    else
                    {
                        return connection;
                    }
                }
                else
                {
                    string url = Protocol + model.LastKnownAddress + _hubLink;
                    logger.Message($"Try do connect to {model.DisplayName} with Address:{url} ({DateTime.Now.ToString()})");
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
                    AfterCreateNewConnection(connection, model);
                    ConnectionChanged?.Invoke(this, model);
                }
                return connection;
            } catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public async Task CloseConnections()
        {
            if (connection != null)
            {
                await connection.StopAsync();
                await connection.DisposeAsync();
                connection = null;
            }
        }

        public virtual void AfterCreateNewConnection(HubConnection connection, Models.Connection model)
        {

        }
    }
}
