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

        public void AutoConnect(Action<bool, SharedCoreModels.Enums.ConnectionState> resultCallback)
        {
            try
            {
                var items = desktopStore.GetItems()?.ToList();
                if (items?.Any(x => x.AutoConnect) == true)
                {
                    int index = 0;
                    OnTryConnect(items, resultCallback, index);
                }
                else
                {
                    resultCallback?.Invoke(false, SharedCoreModels.Enums.ConnectionState.None);
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
                resultCallback?.Invoke(false, SharedCoreModels.Enums.ConnectionState.Error);
            }
        }

        public void Connect(DesktopConfigurationModel desktopConfigurationModel, Action<bool, SharedCoreModels.Enums.ConnectionState> resultCallback)
        {
            OnTryConnect(new List<DesktopConfigurationModel> { desktopConfigurationModel }, resultCallback);
        }

        private void OnTryConnect(List<DesktopConfigurationModel> desktopConfigurations, Action<bool, SharedCoreModels.Enums.ConnectionState> resultCallback, int nextIndex = 0)
        {
            if (desktopConfigurations != null && desktopConfigurations.Count > nextIndex)
            {
                var item = desktopConfigurations[nextIndex];
                communicationHub.Authenticate(deviceData.GetWelcomeDeviceMessage(), item.ConvertToCommunicationConfigurationModel(), (config, result) =>
                {
                    if (result)
                    {
                        communicationHub.Connect(item.ConvertToCommunicationConfigurationModel(), (conResult, state) =>
                        {
                            resultCallback?.Invoke(conResult, state.ConvertToSharedCoreEnum());
                        });
                    }
                    else
                    {
                        nextIndex++;
                        OnTryConnect(desktopConfigurations, resultCallback, nextIndex);
                    }
                }, (manifest) =>
                {
                    entityManager.AddManifestData(manifest, item.Id, item.DisplayName);
                });
            } else
            {
                resultCallback?.Invoke(false, SharedCoreModels.Enums.ConnectionState.Error);
            }
        }
    }
}
