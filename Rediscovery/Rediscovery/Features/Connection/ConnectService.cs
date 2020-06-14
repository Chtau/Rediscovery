using CommunicationBase;
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
        private CommunicationAuthenticationConsumer.IAuthenticationConsumerService authenticationConsumer => DependencyService.Get<CommunicationAuthenticationConsumer.IAuthenticationConsumerService>();
        private CommunicationAuthenticationConsumer.IGreetingConsumerService greetingConsumer => DependencyService.Get<CommunicationAuthenticationConsumer.IGreetingConsumerService>();
        //private CommunicationClientConsumer.IHub communicationHub => DependencyService.Get<CommunicationClientConsumer.IHub>() ?? new CommunicationClientConsumer.Hub();
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();
        private IDeviceData deviceData => DependencyService.Get<IDeviceData>() ?? new DeviceData();
        private Dictionary<Guid, string> desktopConfigurationToken = new Dictionary<Guid, string>();

        public ConnectService()
        {
            //communicationHub.Init(_logger, "/hubs/connect", "/hubs/feature");
        }

        public string GetToken(Guid configurationId)
        {
            if (desktopConfigurationToken.ContainsKey(configurationId))
                return desktopConfigurationToken[configurationId];
            return null;
        }

        private void OnSetToken(Guid configurationId, string token)
        {
            // TODO: remove Token ...
            if (desktopConfigurationToken.ContainsKey(configurationId))
                desktopConfigurationToken[configurationId] = token;
            else
                desktopConfigurationToken.Add(configurationId,token);
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
                var reply = greetingConsumer.GreetHost(item.Address, item.Port, deviceData.GreetingDeviceMessage());
                if (reply.CanConnect == SharedBase.Connection.Enums.AllowConnect.OK)
                {
                    item.PEM = reply.PEM;
                    if (authenticationConsumer.Connect(item.Address, item.SSLPort, item.PEM))
                    {
                        authenticationConsumer.SendWelcome(deviceData.WelcomeDeviceMessage(), deviceReply =>
                        {
                            if (deviceReply.State == SharedBase.Connection.Enums.ConnectionState.OK)
                            {
                                OnSetToken(item.Id, deviceReply.Token);
                                authenticationConsumer.RequestManifest(deviceReply.Token, manifest =>
                                {
                                    entityManager.AddManifestData(manifest, item.Id, item.DisplayName);
                                });
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
                    resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Error);
                }
                
                /*communicationHub.Authenticate(deviceData.GetWelcomeDeviceMessage(), item.ConvertToCommunicationConfigurationModel(), (config, result) =>
                {
                    if (result)
                    {
                        communicationHub.Connect(config, (conResult, state) =>
                        {
                            resultCallback?.Invoke(item, conResult, state.ConvertToSharedCoreEnum());
                        });
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
                            resultCallback?.Invoke(item, result, config.State.ConvertToSharedCoreEnum());
                        }
                    }
                }, (manifest) =>
                {
                    entityManager.AddManifestData(manifest, item.Id, item.DisplayName);
                });*/
            } else
            {
                resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Error);
            }
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
            //communicationHub.Disconnect();
            OnUpdateDesktopConfiguration(desktopConfigurationModel, null, SharedBase.Connection.Enums.ConnectionState.None);
            resultCallback?.Invoke(true);
        }
    }
}
