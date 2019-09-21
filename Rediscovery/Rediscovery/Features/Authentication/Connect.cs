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
        private IDataStoreGuid<Models.ConnectionManifestFeature> connectionManifestFeatureStore => DependencyService.Get<IDataStoreGuid<Models.ConnectionManifestFeature>>() ?? new ConnectionManifestFeatureStore();

        private Dictionary<Guid, HubConnection> connections = new Dictionary<Guid, HubConnection>();

        public event EventHandler<Models.Connection> HelloReceived;
        public event EventHandler<Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>> ManifestReceived;

        public Connect()
        {

        }

        private void OnHello(HubConnection con, Models.Connection model)
        {
            con.On<Enums.ConnectionState, string>("Hello", (state, serverInfo) =>
            {
                logger.Message($"hello received from {model.Name ?? model.Identifies} ({DateTime.Now})");
                model.ConnectionState = state;
                model.LastConnection = DateTime.Now;
                HelloReceived.Invoke(this, model);
            });
        }

        private void OnManifest(HubConnection con, Models.Connection model)
        {
            con.On<Manifest>("Manifest",async (manifest) =>
            {
                logger.Message($"manifest received from {model.Name ?? model.Identifies} ({DateTime.Now})");
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
                        FeatureKey = item
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
            if (model == null)
                return;
            var connection = new HubConnectionBuilder()
                .WithUrl("http://" + model.LastKnownAddress + "/connect")
                .Build();
            if (connections.ContainsKey(model.Id))
                connections[model.Id] = connection;
            else
                connections.Add(model.Id, connection);
            logger.Message($"try connect to {model.Name ?? model.Identifies} ({DateTime.Now})");
            await connections[model.Id].StartAsync();
            OnHello(connections[model.Id], model);
            OnManifest(connections[model.Id], model);
            logger.Message($"send welcome to {model.Name ?? model.Identifies} ({DateTime.Now})");
            await connections[model.Id].InvokeAsync("Welcome", "dev1", model.Id);
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
    }
}
