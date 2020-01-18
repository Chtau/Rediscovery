using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.DesktopConfiguration;
using Rediscovery.Services;
using SharedCoreModels;
using Xamarin.Forms;
using System.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Authentication.Connect))]
namespace Rediscovery.Features.Authentication
{
    public class Connect : IConnect
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDataStoreGuid<Models.Connection> connectionStore => DependencyService.Get<IDataStoreGuid<Models.Connection>>() ?? new ConnectionStore();
        private IDataStoreConnectionGuid<Models.ConnectionManifestFeature> connectionManifestFeatureStore => DependencyService.Get<IDataStoreConnectionGuid<Models.ConnectionManifestFeature>>() ?? new ConnectionManifestFeatureStore();

        private Dictionary<Guid, HubConnection> connections = new Dictionary<Guid, HubConnection>();

        public event EventHandler<Models.Connection> HelloReceived;
        public event EventHandler<Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>> ManifestReceived;
        public event EventHandler<Models.Connection> ConnectionChanged;

        public Connect()
        {

        }

        private void OnHello(HubConnection con, Models.Connection model)
        {
            con.On<Enums.ConnectionState, string>("Hello",(state, token) =>
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
            con.On<Manifest>("Manifest",async (manifest) =>
            {
                logger.Message($"manifest received from {model.DisplayName} ({DateTime.Now})");
                model.ManifestAppMinimumVersion = SharedCoreModels.Version.ConvertFrom(manifest.AppMinimumVersion);
                model.ManifestClientName = manifest.ClientName;
                model.ManifestClientVersion = SharedCoreModels.Version.ConvertFrom(manifest.ClientVersion);
                await connectionStore.UpdateItemAsync(model);
                var features = new List<Models.ConnectionManifestFeature>();
                foreach (var item in manifest.SupportedFeatures)
                {
                    var feature = new Models.ConnectionManifestFeature
                    {
                        ConnectionId = model.Id,
                        FeatureDisplayName = item.DisplayName,
                        FeatureControlIntegrationPoint = item.ControlIntegrationPoint,
                        FeatureFeatureIntegrationPoint = item.FeatureIntegrationPoint,
                        FeatureId = item.Id,
                        FeatureMinControlIntegrationPoint = SharedCoreModels.Version.ConvertFrom(item.MinControlIntegrationPoint),
                        FeatureMinFeatureIntegrationPoint = SharedCoreModels.Version.ConvertFrom(item.MinFeatureIntegrationPoint),
                        FeatureVersion = SharedCoreModels.Version.ConvertFrom(item.Version),
                    };
                    await connectionManifestFeatureStore.AddItemAsync(feature);
                    features.Add(feature);
                }
                ManifestReceived?.Invoke(this, new Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>(model, features));
            });
        }

        public async Task TryConnect(Guid connectionId)
        {
            var model = await connectionStore.GetItemAsync(connectionId);
            await OnTryConnect(model);
        }

        private async Task OnTryConnect(Models.Connection model)
        {
            try
            {
                var con = await OnGetConnection(model);
                if (con != null)
                {
                    logger.Message($"send welcome to {model.DisplayName} ({DateTime.Now})");
                    await con.InvokeAsync("Welcome", model.User);
                }
            } catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public async Task AutoConnect()
        {
            var models = await connectionStore.GetItemsAsync();
            foreach (var item in models.Where(x => x.AutoConnect))
            {
                await OnTryConnect(item);
            }
        }

        public async Task CloseConnections()
        {
            foreach (var item in connections)
            {
                await item.Value.StopAsync();
            }
            connections.Clear();
        }

        public async Task ValidateKey(Guid connectionId, string key)
        {
            try
            {
                var model = await connectionStore.GetItemAsync(connectionId);
                var con = await OnGetConnection(model);
                if (con != null)
                {
                    logger.Message($"send key verify to {model.DisplayName} ({DateTime.Now})");
                    await con.InvokeAsync("AuthorizeKey", model.User, key);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private async Task<HubConnection> OnGetConnection(Models.Connection model)
        {
            if (model == null)
                return null;

            if (connections.ContainsKey(model.Id))
            {
                if (connections[model.Id].State != HubConnectionState.Connected)
                {
                    logger.Message($"remove cached connection {model.DisplayName} ({DateTime.Now})");
                    await connections[model.Id].StopAsync();
                    connections.Remove(model.Id);
                    return await OnGetConnection(model);
                } else
                {
                    return connections[model.Id];
                }
            }
            else
            {
                var connection = new HubConnectionBuilder()
                .WithUrl("http://" + model.LastKnownAddress + "/hubs/connect")
                .Build();

                logger.Message($"try connect to {model.DisplayName} ({DateTime.Now})");
                await connection.StartAsync();
                OnHello(connection, model);
                OnManifest(connection, model);


                var tokenConnection = new HubConnectionBuilder()
                .WithUrl("http://" + model.LastKnownAddress + "/hubs/feature", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(model.Token);
                })
                .Build();
                connections.Add(model.Id, tokenConnection);
                logger.Message($"try token connect to {model.DisplayName} ({DateTime.Now})");
                await connections[model.Id].StartAsync();
                ConnectionChanged?.Invoke(this, model);
            }
            return connections[model.Id];
        }

        public async Task<HubConnection> GetConnection(Models.Connection model)
        {
            return await OnGetConnection(model);
        }
    }
}
