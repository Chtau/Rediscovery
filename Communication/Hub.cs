using CommunicationBase;
using Microsoft.AspNetCore.SignalR.Client;
using PluginFeature.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationResourceConsumer
{
    public class Hub : IHub
    {
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> ActiveDeviceInfoReceived;
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> DeviceInfoReceived;
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> PendingAuthenticationDeviceReceived;
        public event EventHandler<List<SharedCoreModels.DeviceFeature>> ServiceFeatureReceived;
        public event EventHandler<SharedCoreModels.LoggerEntryModel> LogEntryReceived;
        public event EventHandler<bool> ConnectionStateChanged;
        public event EventHandler<SharedCoreModels.EntityContent<Guid, byte[]>> FeatureProfileUIReceived;
        public event EventHandler<SharedCoreModels.EntityContent<Guid, byte[]>> FeatureSettingUIReceived;
        public event EventHandler<SharedCoreModels.EntityContent<Guid, List<DeviceFeatureProfil>>> FeatureProfilesReceived;
        public event EventHandler<SharedCoreModels.EntityContent<Guid, DeviceFeatureSetting>> FeatureSettingsReceived;

        private SharedBase.Logging.ILogger _logger;
        private IConnectionProvider<HubConnection> _connectionProviderAuthentication;
        private IConnectionProvider<HubConnection> _connectionProvider;

        public Hub()
        {
            
        }

        public void Init(SharedBase.Logging.ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP)
        {
            _logger = logger;
            _connectionProviderAuthentication = new ConnectionProviderSignalR();
            _connectionProvider = new ConnectionProviderSignalR();
            _connectionProviderAuthentication.Init(_logger, hubLink, protocol);
            _connectionProvider.Init(_logger, hubLink, protocol);
            _connectionProvider.ConnectionChanged += _connectionProvider_ConnectionChanged;
            _connectionProvider.ConnectionClosed += _connectionProvider_ConnectionClosed;
        }

        private void _connectionProvider_ConnectionClosed(object sender, EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, false);
        }

        private void _connectionProvider_ConnectionChanged(object sender, (ConnectionConfiguration Config, bool IsConnected) e)
        {
            ConnectionStateChanged?.Invoke(this, e.IsConnected);
        }

        public void Authenticate(string applicationKey, ConnectionConfiguration configuration, Action<ConnectionConfiguration, bool> callback)
        {
            Task.Run(async () =>
            {
                await Disconnect();
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
                            _logger.LogError(ex);
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

        public void Connect(string applicationKey, ConnectionConfiguration configuration, Action<bool> listenerCallback)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _connectionProvider.CloseConnection();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                await _connectionProvider.Connect(async (result, connection) =>
                {
                    if (result)
                    {
                        try
                        {
                            connection.On<List<SharedCoreModels.DeviceInfo>>("ActiveDeviceInfo", (deviceInfos) =>
                            {
                                ActiveDeviceInfoReceived?.Invoke(this, deviceInfos);
                                _logger.LogTrace($"[{nameof(ActiveDeviceInfoReceived)}] @{DateTime.Now}");
                            });
                            connection.On<List<SharedCoreModels.DeviceInfo>>("DeviceInfo", (deviceInfos) =>
                            {
                                DeviceInfoReceived?.Invoke(this, deviceInfos);
                                _logger.LogTrace($"[{nameof(DeviceInfoReceived)}] @{DateTime.Now}");
                            });
                            connection.On<List<SharedCoreModels.DeviceInfo>>("PendingAuthenticationDevices", (deviceInfos) =>
                            {
                                PendingAuthenticationDeviceReceived?.Invoke(this, deviceInfos);
                                _logger.LogTrace($"[{nameof(PendingAuthenticationDeviceReceived)}] @{DateTime.Now}");
                            });
                            connection.On<List<SharedCoreModels.DeviceFeature>>("ServiceFeature", (deviceInfos) =>
                            {
                                ServiceFeatureReceived?.Invoke(this, deviceInfos);
                                _logger.LogTrace($"[{nameof(ServiceFeatureReceived)}] @{DateTime.Now}");
                            });
                            connection.On<SharedCoreModels.LoggerEntryModel>("LogEntry", (entry) =>
                            {
                                LogEntryReceived?.Invoke(this, entry);
                            });
                            connection.On<Guid, byte[]>("FeatureDetailsProfilesUI", (featureId, entry) =>
                            {
                                FeatureProfileUIReceived?.Invoke(this, new SharedCoreModels.EntityContent<Guid, byte[]>(featureId, entry));
                                _logger.LogTrace($"[{nameof(FeatureProfileUIReceived)}] @{DateTime.Now}");
                            });
                            connection.On<Guid, byte[]>("FeatureDetailsSettingsUI", (featureId, entry) =>
                            {
                                FeatureSettingUIReceived?.Invoke(this, new SharedCoreModels.EntityContent<Guid, byte[]>(featureId, entry));
                                _logger.LogTrace($"[{nameof(FeatureSettingUIReceived)}] @{DateTime.Now}");
                            });
                            connection.On<Guid, List<DeviceFeatureProfil>>("FeatureDetailsProfiles", (featureId, entry) =>
                            {
                                FeatureProfilesReceived?.Invoke(this, new SharedCoreModels.EntityContent<Guid, List<DeviceFeatureProfil>>(featureId, entry));
                                _logger.LogTrace($"[{nameof(FeatureProfilesReceived)}] @{DateTime.Now}");
                            });
                            connection.On<Guid, DeviceFeatureSetting>("FeatureDetailsSettings", (featureId, entry) =>
                            {
                                FeatureSettingsReceived?.Invoke(this, new SharedCoreModels.EntityContent<Guid, DeviceFeatureSetting>(featureId, entry));
                                _logger.LogTrace($"[{nameof(FeatureSettingsReceived)}] @{DateTime.Now}");
                            });
                            connection.On<bool>("RegisterListenerResponse", (listenerResult) =>
                            {
                                listenerCallback?.Invoke(listenerResult);
                            });
                            await connection.InvokeAsync("RegisterListener", applicationKey);
                        } catch (Exception ex)
                        {
                            _logger.LogError(ex);
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
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestPendingAuthenticationDevices");
                        _logger.LogTrace($"[{nameof(RequestAllData)}] @{DateTime.Now}");
                    } catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
                return true;
            }
            else
                return false;
        }

        public void RequestResolvePendingAuthenticationDevice(Guid deviceId, bool accept)
        {
            if (deviceId != Guid.Empty)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestResolvePendingAuthenticationDevice", deviceId, accept);
                        _logger.LogTrace($"[{nameof(RequestResolvePendingAuthenticationDevice)}] @{DateTime.Now}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
            }
        }

        public void RequestDeleteDevice(SharedCoreModels.DeviceInfo deviceInfo)
        {
            if (deviceInfo != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestDeleteDeviceInfo", deviceInfo);
                        _logger.LogTrace($"[{nameof(RequestDeleteDevice)}] @{DateTime.Now}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
            }
        }

        public void RequestUpdateDevice(SharedCoreModels.DeviceInfo deviceInfo)
        {
            if (deviceInfo != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestUpdateDeviceInfo", deviceInfo);
                        _logger.LogTrace($"[{nameof(RequestUpdateDevice)}] @{DateTime.Now}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
            }
        }

        public async Task<bool> Disconnect()
        {
            try
            {
                if (_connectionProvider != null)
                    await _connectionProvider.CloseConnection();
                if (_connectionProviderAuthentication != null)
                    await _connectionProviderAuthentication.CloseConnection();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        public void RequestFeatureDetails(Guid featureId)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _connectionProvider.CurrentConnection.InvokeAsync("RequestFeatureDetails", featureId);
                    _logger.LogTrace($"[{nameof(RequestFeatureDetails)}] @{DateTime.Now}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
            });
        }

        public void RequestFeatureDetailsUI(Guid featureId)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _connectionProvider.CurrentConnection.InvokeAsync("RequestFeatureDetailsUI", featureId);
                    _logger.LogTrace($"[{nameof(RequestFeatureDetailsUI)}] @{DateTime.Now}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
            });
        }

        public void RequestFeatureSaveProfile(EntityContent<Guid, DeviceFeatureProfil> profileEntity)
        {
            if (profileEntity != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestSaveFeatureProfile", profileEntity.Id, profileEntity.Content);
                        _logger.LogTrace($"[{nameof(RequestFeatureSaveProfile)}] @{DateTime.Now}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
            }
        }

        public void RequestFeatureDeleteProfile(EntityContent<Guid, DeviceFeatureProfil> profileEntity)
        {
            if (profileEntity != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestDeleteFeatureProfile", profileEntity.Id, profileEntity.Content?.Id);
                        _logger.LogTrace($"[{nameof(RequestFeatureDeleteProfile)}] @{DateTime.Now}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
            }
        }

        public void RequestFeatureSaveSetting(EntityContent<Guid, DeviceFeatureSetting> settingEntity)
        {
            if (settingEntity != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestSaveFeatureSettings", settingEntity.Id, settingEntity.Content);
                        _logger.LogTrace($"[{nameof(RequestFeatureSaveSetting)}] @{DateTime.Now}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
            }
        }

        public bool RequestAllFeatures()
        {
            if (_connectionProvider.IsConnected)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("RequestServiceFeature");
                        _logger.LogTrace($"[{nameof(RequestAllFeatures)}] @{DateTime.Now}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
                return true;
            }
            else
                return false;
        }
    }
}
