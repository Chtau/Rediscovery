using CommunicationBase;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationClientConsumer
{
    public class Hub : IHub
    {
        private ILogger _logger;
        private IConnectionProvider<HubConnection> _connectionProviderAuthentication;
        private IConnectionProvider<HubConnection> _connectionProvider;

        public void Init(ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP)
        {
            _logger = logger;
            _connectionProviderAuthentication = new ConnectionProviderSignalR();
            _connectionProvider = new ConnectionProviderSignalR();
            _connectionProviderAuthentication.Init(_logger, hubLink, protocol);
            _connectionProvider.Init(_logger, hubLink, protocol);
        }

        public void Authenticate(string deviceIdentifier, ConnectionConfiguration configuration, Action<ConnectionConfiguration, bool> callback)
        {
            Disconnect();
            Task.Run(async () =>
            {
                await _connectionProviderAuthentication.Connect(async (result, connection) =>
                {
                    if (result)
                    {
                        try
                        {
                            connection.On<string>("Hello", (token) =>
                            {
                                if (!string.IsNullOrWhiteSpace(token))
                                {
                                    configuration.Token = token;
                                    callback.Invoke(configuration, true);
                                }
                                else
                                {
                                    configuration.Token = null;
                                    callback.Invoke(configuration, false);
                                }
                            });
                            await connection.InvokeAsync("Hello", deviceIdentifier);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                            configuration.Token = null;
                            callback.Invoke(configuration, false);
                        }
                    }
                    else
                    {
                        configuration.Token = null;
                        callback.Invoke(configuration, false);
                    }
                }, configuration, false);
            });
        }

        public void Connect(string deviceIdentifier, ConnectionConfiguration configuration, Action<bool> listenerCallback)
        {
            try
            {
                _connectionProvider.CloseConnection();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            Task.Run(async () =>
            {
                await _connectionProvider.Connect(async (result, connection) =>
                {
                    if (result)
                    {
                        try
                        {
                            
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                }, configuration, true);
            });
        }

        public void Disconnect()
        {
            try
            {
                _connectionProvider.CloseConnection();
                _connectionProviderAuthentication.CloseConnection();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
