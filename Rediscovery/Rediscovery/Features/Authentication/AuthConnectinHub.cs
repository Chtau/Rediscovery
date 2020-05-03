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
using Rediscovery.Features.Connection;

namespace Rediscovery.Features.Authentication
{
    public class AuthConnectinHub : InternalHubs, IInternalHub
    {
        const string AuthHubLink = "/hubs/connect";

        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();
        private IDataStoreGuid<Settings.Models.SettingModel> settingStore => DependencyService.Get<IDataStoreGuid<Settings.Models.SettingModel>>() ?? new Settings.SettingStore();
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();

        public event EventHandler<DesktopConfiguration.DesktopConfigurationModel> HelloReceived;
        public event EventHandler<Tuple<DesktopConfiguration.DesktopConfigurationModel, List<Connection.Models.ConnectionManifestFeature>>> ManifestReceived;

        public AuthConnectinHub(): base(AuthHubLink)
        {

        }

        public async Task<HubConnection> GetConnection(DesktopConfiguration.DesktopConfigurationModel model)
        {
            return await base.OnGetConnection(model, false);
        }

        public override void AfterCreateNewConnection(HubConnection connection, DesktopConfiguration.DesktopConfigurationModel model)
        {
            base.AfterCreateNewConnection(connection, model);
            OnHello(connection, model);
            OnManifest(connection, model);
            _logger.Message($"Send Welcome to Service:{model.DisplayName} ({DateTime.Now})");
            var setting = settingStore.GetItem(Guid.Empty);
            connection.InvokeAsync("Welcome", setting.DeviceIdentifier);
        }

        private void OnHello(HubConnection con, DesktopConfiguration.DesktopConfigurationModel model)
        {
            con.On<Enums.ConnectionState, string>("Hello", (state, token) =>
            {
                _logger.Message($"hello received from {model.DisplayName} ({DateTime.Now})");
                model.ConnectionState = state;
                model.LastConnection = DateTime.Now;
                model.Token = token;
                Task.Run(async () =>
                {
                    try
                    {
                        await desktopStore.UpdateItemAsync(model);
                        HelloReceived?.Invoke(this, model);
                    } catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.ToString());
                    }
                });
            });
        }

        private void OnManifest(HubConnection con, DesktopConfiguration.DesktopConfigurationModel model)
        {
            con.On<Manifest>("Manifest", async (manifest) =>
            {
                _logger.Message($"manifest received from {model.DisplayName} ({DateTime.Now})");
                model.ManifestAppMinimumVersion = PluginFeature.Models.Version.ConvertFrom(manifest.AppMinimumVersion);
                model.ManifestClientName = manifest.ClientName;
                model.ManifestClientVersion = PluginFeature.Models.Version.ConvertFrom(manifest.ClientVersion);
                await desktopStore.UpdateItemAsync(model);
                var features = new List<Connection.Models.ConnectionManifestFeature>();
                foreach (var item in manifest.SupportedFeatures)
                {
                    var feature = new Connection.Models.ConnectionManifestFeature
                    {
                        ConnectionId = model.Id,
                        ConnectionDisplayName = model.DisplayName,
                        FeatureDisplayName = item.DisplayName,
                        FeatureControlIntegrationPoint = item.ControlIntegrationPoint,
                        FeatureFeatureIntegrationPoint = item.FeatureIntegrationPoint,
                        ControlIntegration = item.ControlIntegration,
                        FeatureId = item.Id,
                        FeatureMinControlIntegrationPoint = PluginFeature.Models.Version.ConvertFrom(item.MinControlIntegrationPoint),
                        FeatureMinFeatureIntegrationPoint = PluginFeature.Models.Version.ConvertFrom(item.MinFeatureIntegrationPoint),
                        FeatureVersion = PluginFeature.Models.Version.ConvertFrom(item.Version),
                    };
                    features.Add(feature);
                    entityManager.ConnectionManifestFeatures.Add(feature);
                }
                ManifestReceived?.Invoke(this, new Tuple<DesktopConfiguration.DesktopConfigurationModel, List<Connection.Models.ConnectionManifestFeature>>(model, features));
            });
        }
    }
}
