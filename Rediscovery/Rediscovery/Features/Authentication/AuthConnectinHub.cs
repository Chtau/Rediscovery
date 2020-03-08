using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.Features.Authentication.Models;
using Rediscovery.Features.DesktopFeatures;
using Rediscovery.Services;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace Rediscovery.Features.Authentication
{
    public class AuthConnectinHub : InternalHubs, IInternalHub
    {
        const string AuthHubLink = "/hubs/connect";

        private IDataStoreGuid<Models.Connection> connectionStore => DependencyService.Get<IDataStoreGuid<Models.Connection>>() ?? new ConnectionStore();
        private IEntityManager entityManager => DependencyService.Get<IEntityManager>() ?? new EntityManager();
        private DesktopFeatures.IFeatureUIService featureUIService => DependencyService.Get<DesktopFeatures.IFeatureUIService>() ?? new DesktopFeatures.FeatureUIService();

        public event EventHandler<Models.Connection> HelloReceived;
        public event EventHandler<Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>> ManifestReceived;

        public AuthConnectinHub(): base(AuthHubLink)
        {

        }

        public async Task<HubConnection> GetConnection(Models.Connection model)
        {
            return await base.OnGetConnection(model, false);
        }

        public override void AfterCreateNewConnection(HubConnection connection, Connection model)
        {
            base.AfterCreateNewConnection(connection, model);
            OnHello(connection, model);
            OnManifest(connection, model);
            logger.Message($"Send Welcome to Service:{model.DisplayName} ({DateTime.Now})");
            connection.InvokeAsync("Welcome", model.User);
        }

        private void OnHello(HubConnection con, Models.Connection model)
        {
            con.On<Enums.ConnectionState, string>("Hello", (state, token) =>
            {
                logger.Message($"hello received from {model.DisplayName} ({DateTime.Now})");
                model.ConnectionState = state;
                model.LastConnection = DateTime.Now;
                model.Token = token;
                Task.Run(async () =>
                {
                    await connectionStore.UpdateItemAsync(model);
                    HelloReceived?.Invoke(this, model);
                });
            });
        }

        private void OnManifest(HubConnection con, Models.Connection model)
        {
            con.On<Manifest>("Manifest", async (manifest) =>
            {
                logger.Message($"manifest received from {model.DisplayName} ({DateTime.Now})");
                model.ManifestAppMinimumVersion = PluginFeature.Models.Version.ConvertFrom(manifest.AppMinimumVersion);
                model.ManifestClientName = manifest.ClientName;
                model.ManifestClientVersion = PluginFeature.Models.Version.ConvertFrom(manifest.ClientVersion);
                await connectionStore.UpdateItemAsync(model);
                var features = new List<Models.ConnectionManifestFeature>();
                foreach (var item in manifest.SupportedFeatures)
                {
                    try
                    {
                        if (item.UIZipArchive != null)
                            featureUIService.SaveUI(item.UIZipArchive as System.IO.Compression.ZipArchive, item.Id);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                    System.Collections.ObjectModel.ObservableCollection<SharedCoreModels.DeviceFeature.DeviceFeatureProfil> profiles = new System.Collections.ObjectModel.ObservableCollection<SharedCoreModels.DeviceFeature.DeviceFeatureProfil>();
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(item.Profiles))
                        {
                            var profs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SharedCoreModels.DeviceFeature.DeviceFeatureProfil>>(item.Profiles);
                            if (profs != null)
                                profiles = new System.Collections.ObjectModel.ObservableCollection<SharedCoreModels.DeviceFeature.DeviceFeatureProfil>(profs);
                        }
                    } catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                    var feature = new Models.ConnectionManifestFeature
                    {
                        ConnectionId = model.Id,
                        FeatureDisplayName = item.DisplayName,
                        FeatureControlIntegrationPoint = item.ControlIntegrationPoint,
                        FeatureFeatureIntegrationPoint = item.FeatureIntegrationPoint,
                        ControlIntegration = item.ControlIntegration,
                        FeatureId = item.Id,
                        FeatureMinControlIntegrationPoint = PluginFeature.Models.Version.ConvertFrom(item.MinControlIntegrationPoint),
                        FeatureMinFeatureIntegrationPoint = PluginFeature.Models.Version.ConvertFrom(item.MinFeatureIntegrationPoint),
                        FeatureVersion = PluginFeature.Models.Version.ConvertFrom(item.Version),
                        SettingsObject = item.SettingsObject,
                        Profiles = profiles
                    };
                    features.Add(feature);
                    entityManager.ConnectionManifestFeatures.Add(feature);
                }
                ManifestReceived?.Invoke(this, new Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>(model, features));
            });
        }
    }
}
