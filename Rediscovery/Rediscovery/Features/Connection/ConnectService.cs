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
        private CommunicationClientConsumer.IHub communicationHub => DependencyService.Get<CommunicationClientConsumer.IHub>() ?? new CommunicationClientConsumer.Hub();
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();
        private IDeviceData deviceData => DependencyService.Get<IDeviceData>() ?? new DeviceData();

        public ConnectService()
        {
            communicationHub.Init(_logger, "/hubs/connect", "/hubs/feature");
        }

        public void AutoConnect(Action<bool, SharedCoreModels.Enums.ConnectionState> resultCallback)
        {
            OnResetDesktopConfigurationState();
            Action<DesktopConfigurationModel, bool, SharedCoreModels.Enums.ConnectionState> callback = (config, result, state) =>
            {
                OnUpdateDesktopConfiguration(config, result, state);
                resultCallback?.Invoke(result, state);
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
                    callback?.Invoke(null, false, SharedCoreModels.Enums.ConnectionState.None);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                callback?.Invoke(null, false, SharedCoreModels.Enums.ConnectionState.Error);
            }
        }

        public void Connect(DesktopConfigurationModel desktopConfigurationModel, Action<bool, SharedCoreModels.Enums.ConnectionState> resultCallback)
        {
            OnResetDesktopConfigurationState();
            Action<DesktopConfigurationModel, bool, SharedCoreModels.Enums.ConnectionState> callback = (config, result, state) =>
            {
                OnUpdateDesktopConfiguration(desktopConfigurationModel, result, state);
                resultCallback?.Invoke(result, state);
            };
            OnTryConnect(new List<DesktopConfigurationModel> { desktopConfigurationModel }, callback);
        }

        private void OnTryConnect(List<DesktopConfigurationModel> desktopConfigurations, Action<DesktopConfigurationModel, bool, SharedCoreModels.Enums.ConnectionState> resultCallback, int nextIndex = 0)
        {
            if (desktopConfigurations != null && desktopConfigurations.Count > nextIndex)
            {
                var item = desktopConfigurations[nextIndex];
                communicationHub.Authenticate(deviceData.GetWelcomeDeviceMessage(), item.ConvertToCommunicationConfigurationModel(), (config, result) =>
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
                });
            } else
            {
                resultCallback?.Invoke(null, false, SharedCoreModels.Enums.ConnectionState.Error);
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
                        item.ConnectionState = SharedCoreModels.Enums.ConnectionState.None;
                        desktopStore.UpdateItem(item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void OnUpdateDesktopConfiguration(DesktopConfigurationModel configuration, bool result, SharedCoreModels.Enums.ConnectionState state)
        {
            try
            {
                if (configuration != null)
                {
                    configuration.ConnectionState = state;
                    if (result)
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
            communicationHub.Disconnect();
            OnUpdateDesktopConfiguration(desktopConfigurationModel, true, SharedCoreModels.Enums.ConnectionState.None);
            resultCallback?.Invoke(true);
        }
    }
}
