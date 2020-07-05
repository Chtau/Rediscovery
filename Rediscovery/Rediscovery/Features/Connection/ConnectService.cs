using CommunicationBase;
using Rediscovery.Features.Connection.Models;
using Rediscovery.Features.DesktopConfiguration;
using Rediscovery.Services;
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
        private Dictionary<Guid, ConnectConfigurationData> desktopConfigurationData = new Dictionary<Guid, ConnectConfigurationData>();

        public ConnectService()
        {
            
        }

        public ConnectConfigurationData GetData(Guid configurationId)
        {
            if (desktopConfigurationData.ContainsKey(configurationId))
                return desktopConfigurationData[configurationId];
            return null;
        }

        private void OnSetData(Guid configurationId, ConnectConfigurationData data)
        {
            if (desktopConfigurationData.ContainsKey(configurationId))
                desktopConfigurationData[configurationId] = data;
            else
                desktopConfigurationData.Add(configurationId, data);
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
                var reply = consumer.GreetingConsumerService.GreetHost(item.Address, item.Port, deviceData.GreetingDeviceMessage(), setting.ConnectTimeout);
                if (reply.CanConnect == SharedBase.Connection.Enums.AllowConnect.OK)
                {
                    if (consumer.AuthenticationConsumerService.Connect(item.Address, reply.SSLPort, reply.PEM))
                    {
                        consumer.AuthenticationConsumerService.SendWelcome(deviceData.WelcomeDeviceMessage(), deviceReply =>
                        {
                            if (deviceReply.State == SharedBase.Connection.Enums.ConnectionState.OK)
                            {
                                OnSetData(item.Id, new ConnectConfigurationData
                                {
                                    Token = deviceReply.Token,
                                    PEM = reply.PEM,
                                    SSLPort = reply.SSLPort
                                });
                                consumer.AuthenticationConsumerService.RequestManifest(deviceReply.Token, manifest =>
                                {
                                    entityManager.AddManifestData(manifest, item.Id, item.DisplayName);
                                });
                                OnConnectHeartbeat(item.Address, reply.SSLPort, reply.PEM, deviceReply.Token);
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

        private void OnConnectHeartbeat(string ipAddress, int port, string pem, string token)
        {
            try
            {
                if (consumer.HeartbeatConsumerService.Connect(ipAddress, port, pem))
                {
                    consumer.HeartbeatConsumerService.ReceivedBeatRoundtrip += HeartbeatConsumerService_ReceivedBeatRoundtrip;
                    consumer.HeartbeatConsumerService.StartBeat(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void HeartbeatConsumerService_ReceivedBeatRoundtrip(object sender, CommunicationHeartbeatConsumer.RoundTripResult e)
        {
            _logger.LogTrace($"[Heartbeat] round trip received. ({e.PingPongTime?.TotalMilliseconds} ms)");
        }

        private void OnResetDesktopConfigurationState()
        {
            try
            {
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
            consumer.Disconnect();
            OnUpdateDesktopConfiguration(desktopConfigurationModel, null, SharedBase.Connection.Enums.ConnectionState.None);
            resultCallback?.Invoke(true);
        }
    }
}
