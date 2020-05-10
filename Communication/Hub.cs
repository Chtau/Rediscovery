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

        private readonly ILogger _logger;
        private readonly Internal.IConnectionProvider<HubConnection> _connectionProviderAuthentication;
        private readonly Internal.IConnectionProvider<HubConnection> _connectionProvider;

        public Hub(ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP)
        {
            _logger = logger;
            _connectionProviderAuthentication = new Internal.ConnectionProviderSignalR();
            _connectionProvider = new Internal.ConnectionProviderSignalR();
            _connectionProviderAuthentication.Init(_logger, hubLink, protocol);
            _connectionProvider.Init(_logger, hubLink, protocol);
        }

        public void Authenticate(string applicationKey, Models.ConnectionConfiguration configuration, Action<Models.ConnectionConfiguration, bool> callback)
        {
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
                            if (connection.State != HubConnectionState.Connected)
                            {
                                await connection.StartAsync();
                            }
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

        public void Connect(string applicationKey, Models.ConnectionConfiguration configuration)
        {
            Task.Run(async () =>
            {
                await _connectionProviderAuthentication.Connect(async (result, connection) =>
                {
                    if (result)
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
                        await connection.InvokeAsync("RegisterListener", applicationKey);
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
                    await _connectionProvider.CurrentConnection.InvokeAsync("RequestDeviceInfo");
                    await _connectionProvider.CurrentConnection.InvokeAsync("RequestServiceFeature");
                    await _connectionProvider.CurrentConnection.InvokeAsync("RequestActiveDeviceInfo");
                });
                return true;
            }
            else
                return false;
        }
    }
}
