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

        public void AutoConnect(Action<SharedCoreModels.Enums.ConnectionState> resultCallback)
        {
            try
            {
                var items = desktopStore.GetItems()?.ToList();
                if (items?.Any(x => x.AutoConnect) == true)
                {
                    int index = 0;
                    var item = items[index];
                    communicationHub.Authenticate(deviceData.GetWelcomeDeviceMessage(), item.ConvertToCommunicationConfigurationModel(), (config, result) =>
                    {
                        if (result)
                        {
                            communicationHub.Connect(deviceData.GetDeviceIdentifier(), item.ConvertToCommunicationConfigurationModel(), (conResult) =>
                            {
                                if (!conResult)
                                {
                                    resultCallback?.Invoke(SharedCoreModels.Enums.ConnectionState.Warning);
                                } else
                                {
                                    resultCallback?.Invoke(SharedCoreModels.Enums.ConnectionState.OK);
                                }
                            });
                        }
                        else
                        {
                            // TODO: recursive call to connect to the next item
                            resultCallback?.Invoke(config.State.ConvertToSharedCoreEnum());
                        }
                    }, (manifest) =>
                    {
                        entityManager.AddManifestData(manifest, item.Id, item.DisplayName);
                    });
                }
                else
                {
                    resultCallback?.Invoke(SharedCoreModels.Enums.ConnectionState.None);
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
                resultCallback?.Invoke(SharedCoreModels.Enums.ConnectionState.Error);
            }
        }

        public void Connect(DesktopConfigurationModel desktopConfigurationModel, Action<SharedCoreModels.Enums.ConnectionState> resultCallback)
        {
            throw new NotImplementedException();
        }
    }
}
