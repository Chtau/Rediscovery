using CommunicationBase;
using Rediscovery.Features.Connection.Models;
using Rediscovery.Features.DesktopConfiguration;
using Rediscovery.Services;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Connection.ConnectService))]
namespace Rediscovery.Features.Connection
{
    public class ConnectService : BaseService, IConnectService
    {
        private IConsumer consumer => DependencyService.Get<IConsumer>();
        private IDataStoreGuid<Features.Settings.Models.SettingModel> settingStore => DependencyService.Get<IDataStoreGuid<Features.Settings.Models.SettingModel>>() ?? new Features.Settings.SettingStore();
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();
        private IDeviceData deviceData => DependencyService.Get<IDeviceData>() ?? new DeviceData();
        private ILoggerEvent loggerEvent => DependencyService.Get<ILoggerEvent>() ?? new Services.Logger();

        private readonly Dictionary<Guid, ConnectConfigurationData> _desktopConfigurationData = new Dictionary<Guid, ConnectConfigurationData>();
        private readonly Dictionary<Guid, CommunicationHeartbeatConsumer.RoundTripResult> _lastHeartbeatStates = new Dictionary<Guid, CommunicationHeartbeatConsumer.RoundTripResult>();

        public event EventHandler<Guid> HeartbeatStateChanges;

        public ConnectService()
        {
            loggerEvent.EntryAdded += LoggerEvent_EntryAdded;
        }

        private void LoggerEvent_EntryAdded(object sender, LoggerEntry e)
        {
            consumer?.LoggerConsumer?.LogEntry(e);
        }

        public CommunicationHeartbeatConsumer.RoundTripResult GetHeartbeat(Guid desktopConfigurationId)
        {
            if (_lastHeartbeatStates.ContainsKey(desktopConfigurationId))
                return _lastHeartbeatStates[desktopConfigurationId];
            return new CommunicationHeartbeatConsumer.RoundTripResult(desktopConfigurationId.ToString(), false);
        }

        public ConnectConfigurationData GetData(Guid configurationId)
        {
            if (_desktopConfigurationData.ContainsKey(configurationId))
                return _desktopConfigurationData[configurationId];
            return null;
        }

        private void OnSetData(Guid configurationId, ConnectConfigurationData data)
        {
            if (_desktopConfigurationData.ContainsKey(configurationId))
                _desktopConfigurationData[configurationId] = data;
            else
                _desktopConfigurationData.Add(configurationId, data);
        }

        public void AutoConnect(Action<string, SharedBase.Connection.Enums.ConnectionState> resultCallback)
        {
            OnResetDesktopConfigurationState();
            Action<DesktopConfigurationModel, string, SharedBase.Connection.Enums.ConnectionState> callback = (config, token, state) =>
            {
                OnUpdateDesktopConfiguration(config, token, state);
                resultCallback?.Invoke(token, state);
            };
            try
            {
                int index = 0;
                var items = desktopStore.GetItems()?.ToList();
                if (items?.Any(x => x.AutoConnect) == true)
                {
                    OnTryConnect(items, callback, index);
                }
                else
                {
                    callback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.None);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                callback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Error);
            }
        }

        public void Connect(DesktopConfigurationModel desktopConfigurationModel, Action<string, SharedBase.Connection.Enums.ConnectionState> resultCallback)
        {
            OnResetDesktopConfigurationState();
            Action<DesktopConfigurationModel, string, SharedBase.Connection.Enums.ConnectionState> callback = (config, token, state) =>
            {
                OnUpdateDesktopConfiguration(desktopConfigurationModel, token, state);
                resultCallback?.Invoke(token, state);
            };
            OnTryConnect(new List<DesktopConfigurationModel> { desktopConfigurationModel }, callback);
        }

        private void OnTryConnect(List<DesktopConfigurationModel> desktopConfigurations, Action<DesktopConfigurationModel, string, SharedBase.Connection.Enums.ConnectionState> resultCallback, int nextIndex = 0)
        {
            if (desktopConfigurations != null && desktopConfigurations.Count > nextIndex)
            {
                var item = desktopConfigurations[nextIndex];
                var setting = settingStore.GetItem(Guid.Empty);
                var reply = consumer.GreetingConsumerService.GreetHost(item.Address, item.Port, deviceData.GreetingDeviceMessage(), setting == null ? 2 : setting.ConnectTimeout);
                if (reply.CanConnect == SharedBase.Connection.Enums.AllowConnect.OK)
                {
                    CommunicationBase.ConsumerConnectionConfiguration connectionConfiguration = new ConsumerConnectionConfiguration
                    {
                        UseSSL = reply.UseSSL,
                        CertificatePEM = reply.PEM,
                        IPAddress = item.Address,
                        Port = item.Port,
                        SSLPort = reply.SSLPort
                    };
                    if (consumer.AuthenticationConsumerService.Connect(connectionConfiguration))
                    {
                        consumer.AuthenticationConsumerService.SendWelcome(deviceData.WelcomeDeviceMessage(), deviceReply =>
                        {
                            if (deviceReply.State == SharedBase.Connection.Enums.ConnectionState.OK)
                            {
                                OnSetData(item.Id, new ConnectConfigurationData
                                {
                                    Token = deviceReply.Token,
                                    PEM = reply.PEM,
                                    SSLPort = reply.SSLPort,
                                    UseSSL = reply.UseSSL,
                                    Port = item.Port,
                                });
                                consumer.AuthenticationConsumerService.RequestManifest(deviceReply.Token, manifest => entityManager.AddManifestData(manifest, item.Id, item.DisplayName));
                                OnConnectLogger(connectionConfiguration, deviceReply.Token);
                                OnConnectHeartbeat(connectionConfiguration, deviceReply.Token, item.Id);
                                resultCallback?.Invoke(item, deviceReply.Token, deviceReply.State);
                            }
                            else
                            {
                                nextIndex++;
                                if (desktopConfigurations.Count > nextIndex)
                                {
                                    OnTryConnect(desktopConfigurations, resultCallback, nextIndex);
                                }
                                else
                                {
                                    if (item.ConnectionState == SharedBase.Connection.Enums.ConnectionState.None)
                                        item.ConnectionState = deviceReply.State;
                                    resultCallback?.Invoke(item, null, item.ConnectionState);
                                }
                            }
                        });
                    }
                    else
                    {
                        resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Error);
                    }
                } else
                {
                    if (reply.Offline)
                        resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Offline);
                    else
                        resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Error);
                }
            } else
            {
                resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Error);
            }
        }

        private void OnConnectHeartbeat(CommunicationBase.ConsumerConnectionConfiguration connectionConfiguration, string token, Guid desktopConfigurationId)
        {
            try
            {
                if (consumer.HeartbeatConsumerService.Connect(connectionConfiguration))
                {
                    consumer.HeartbeatConsumerService.ReceivedBeatRoundtrip += HeartbeatConsumerService_ReceivedBeatRoundtrip;
                    consumer.HeartbeatConsumerService.StartBeat(desktopConfigurationId.ToString(), token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void HeartbeatConsumerService_ReceivedBeatRoundtrip(object sender, CommunicationHeartbeatConsumer.RoundTripResult e)
        {
            try
            {
                if (e.OK)
                    _logger.LogTrace($"[Heartbeat] round trip received. ({e.PingPongTime?.TotalMilliseconds} ms)");
                else
                    _logger.LogTrace("[Heartbeat] round trip abort received.");

                if (!string.IsNullOrWhiteSpace(e.Identifier) && Guid.TryParse(e.Identifier, out Guid desktopConfigurationId))
                {
                    bool shouldUpdate = false;
                    if (_lastHeartbeatStates.ContainsKey(desktopConfigurationId))
                    {
                        if (_lastHeartbeatStates[desktopConfigurationId]?.OK != e.OK)
                        {
                            _lastHeartbeatStates[desktopConfigurationId].OK = e.OK;
                            shouldUpdate = true;
                        }
                    } else
                    {
                        _lastHeartbeatStates.Add(desktopConfigurationId, e);
                        shouldUpdate = true;
                    }
                    if (shouldUpdate)
                    {
                        var item = desktopStore.GetItem(desktopConfigurationId);
                        item.ConnectionState = e.OK ? SharedBase.Connection.Enums.ConnectionState.OK : SharedBase.Connection.Enums.ConnectionState.None;
                        desktopStore.UpdateItem(item);
                    }
                    _lastHeartbeatStates[desktopConfigurationId].PingPongTime = e.PingPongTime;
                    _lastHeartbeatStates[desktopConfigurationId].PingStartDatetimeUTC = e.PingStartDatetimeUTC;
                    HeartbeatStateChanges?.Invoke(this, desktopConfigurationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void OnConnectLogger(CommunicationBase.ConsumerConnectionConfiguration connectionConfiguration, string token)
        {
            try
            {
                if (consumer.LoggerConsumer.Connect(connectionConfiguration))
                {
                    consumer.LoggerConsumer.StartLogger(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void OnResetDesktopConfigurationState()
        {
            try
            {
                _lastHeartbeatStates.Clear();
                var items = desktopStore.GetItems()?.ToList();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        item.ConnectionState = SharedBase.Connection.Enums.ConnectionState.None;
                        desktopStore.UpdateItem(item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void OnUpdateDesktopConfiguration(DesktopConfigurationModel configuration, string token, SharedBase.Connection.Enums.ConnectionState state)
        {
            try
            {
                if (configuration != null)
                {
                    configuration.ConnectionState = state;
                    if (configuration.ConnectionState == SharedBase.Connection.Enums.ConnectionState.OK)
                        configuration.LastConnection = DateTime.Now;
                    desktopStore.UpdateItem(configuration);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void Disconnect(DesktopConfigurationModel desktopConfigurationModel, Action<bool> resultCallback)
        {
            _lastHeartbeatStates.Clear();
            consumer.Disconnect();
            OnUpdateDesktopConfiguration(desktopConfigurationModel, null, SharedBase.Connection.Enums.ConnectionState.None);
            resultCallback?.Invoke(true);
        }

        public void InvokeLogEntry(LoggerEntry loggerEntry)
        {
            try
            {
                consumer?.LoggerConsumer?.LogEntry(loggerEntry);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                //_logger.LogError(ex);
            }
        }
    }
}
