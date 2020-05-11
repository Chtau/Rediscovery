using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationConsumer
{
    public class Hub : IHub
    {
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> ActiveDeviceInfoReceived;
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> DeviceInfoReceived;
        public event EventHandler<List<SharedCoreModels.DeviceFeature>> ServiceFeatureReceived;
        public event EventHandler<SharedCoreModels.LoggerEntryModel> LogEntryReceived;

        private ILogger _logger;
        private Internal.IConnectionProvider<HubConnection> _connectionProviderAuthentication;
        private Internal.IConnectionProvider<HubConnection> _connectionProvider;

        public Hub()
        {
            
        }

        public void Init(ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP)
        {
            _logger = logger;
            _connectionProviderAuthentication = new Internal.ConnectionProviderSignalR();
            _connectionProvider = new Internal.ConnectionProviderSignalR();
            _connectionProviderAuthentication.Init(_logger, hubLink, protocol);
            _connectionProvider.Init(_logger, hubLink, protocol);
        }

        public void Authenticate(string applicationKey, Models.ConnectionConfiguration configuration, Action<Models.ConnectionConfiguration, bool> callback)
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
                            await connection.InvokeAsync("Hello", applicationKey);
                        } catch (Exception ex)
                        {
                            _logger.Error(ex);
                            configuration.Token = null;
                            callback.Invoke(configuration, false);
                        }
                    } else
                    {
                        configuration.Token = null;
                        callback.Invoke(configuration, false);
                    }
                }, configuration, false);
            });
        }

        public void Connect(string applicationKey, Models.ConnectionConfiguration configuration, Action<bool> listenerCallback)
        {
            try
            {
                _connectionProvider.CloseConnection();
            } catch (Exception ex)
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
                            connection.On<List<SharedCoreModels.DeviceInfo>>("ActiveDeviceInfo", (deviceInfos) =>
                            {
                                ActiveDeviceInfoReceived?.Invoke(this, deviceInfos);
                            });
                            connection.On<List<SharedCoreModels.DeviceInfo>>("DeviceInfo", (deviceInfos) =>
                            {
                                DeviceInfoReceived?.Invoke(this, deviceInfos);
                            });
                            connection.On<List<SharedCoreModels.DeviceFeature>>("ServiceFeature", (deviceInfos) =>
                            {
                                ServiceFeatureReceived?.Invoke(this, deviceInfos);
                            });
                            connection.On<SharedCoreModels.LoggerEntryModel>("LogEntry", (entry) =>
                            {
                                LogEntryReceived?.Invoke(this, entry);
                            });
                            connection.On<bool>("RegisterListenerResponse", (listenerResult) =>
                            {
                                listenerCallback?.Invoke(listenerResult);
                            });
                            await connection.InvokeAsync("RegisterListener", applicationKey);
                        } catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                }, configuration, true);
            });
        }

        public bool RequestAllData()
        {
            if (_connectionProvider.IsConnected)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestDeviceInfo");
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestServiceFeature");
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestActiveDeviceInfo");
                    } catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                });
                return true;
            }
            else
                return false;
        }

        public void Disconnect()
        {
            try
            {
                _connectionProvider.CloseConnection();
                _connectionProviderAuthentication.CloseConnection();
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
