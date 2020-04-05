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

        private IDataStoreGuid<Connection.Models.ConnectionInfo> connectionStore => DependencyService.Get<IDataStoreGuid<Connection.Models.ConnectionInfo>>() ?? new ConnectionStore();
        private IEntityManager entityManager => DependencyService.Get<IEntityManager>() ?? new EntityManager();

        public event EventHandler<Connection.Models.ConnectionInfo> HelloReceived;
        public event EventHandler<Tuple<Connection.Models.ConnectionInfo, List<Connection.Models.ConnectionManifestFeature>>> ManifestReceived;

        public AuthConnectinHub(): base(AuthHubLink)
        {

        }

        public async Task<HubConnection> GetConnection(Connection.Models.ConnectionInfo model)
        {
            return await base.OnGetConnection(model, false);
        }

        public override void AfterCreateNewConnection(HubConnection connection, Connection.Models.ConnectionInfo model)
        {
            base.AfterCreateNewConnection(connection, model);
            OnHello(connection, model);
            OnManifest(connection, model);
            _logger.Message($"Send Welcome to Service:{model.DisplayName} ({DateTime.Now})");
            connection.InvokeAsync("Welcome", model.User);
        }

        private void OnHello(HubConnection con, Connection.Models.ConnectionInfo model)
        {
            con.On<Enums.ConnectionState, string>("Hello", (state, token) =>
            {
                _logger.Message($"hello received from {model.DisplayName} ({DateTime.Now})");
                model.ConnectionState = state;
                model.LastConnection = DateTime.Now;
                model.Token = token;
                _logger.Message($"OnHello=>New Token received: Token={token}");
                Task.Run(async () =>
                {
                    await connectionStore.UpdateItemAsync(model);
                    HelloReceived?.Invoke(this, model);
                });
            });
        }

        private void OnManifest(HubConnection con, Connection.Models.ConnectionInfo model)
        {
            con.On<Manifest>("Manifest", async (manifest) =>
            {
                _logger.Message($"manifest received from {model.DisplayName} ({DateTime.Now})");
                model.ManifestAppMinimumVersion = PluginFeature.Models.Version.ConvertFrom(manifest.AppMinimumVersion);
                model.ManifestClientName = manifest.ClientName;
                model.ManifestClientVersion = PluginFeature.Models.Version.ConvertFrom(manifest.ClientVersion);
                await connectionStore.UpdateItemAsync(model);
                var features = new List<Connection.Models.ConnectionManifestFeature>();
                foreach (var item in manifest.SupportedFeatures)
                {
                    var feature = new Connection.Models.ConnectionManifestFeature
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
                    };
                    features.Add(feature);
                    entityManager.ConnectionManifestFeatures.Add(feature);
                }
                ManifestReceived?.Invoke(this, new Tuple<Connection.Models.ConnectionInfo, List<Connection.Models.ConnectionManifestFeature>>(model, features));
            });
        }
    }
}
