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
        public enum HubTypes
        {
            Auth,
            Feature,
        }

        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDataStoreGuid<Models.Connection> connectionStore => DependencyService.Get<IDataStoreGuid<Models.Connection>>() ?? new ConnectionStore();

        private Dictionary<Guid, IInternalHub> authHubs = new Dictionary<Guid, IInternalHub>();
        private Dictionary<Guid, IInternalHub> featureHubs = new Dictionary<Guid, IInternalHub>();

        public event EventHandler<Models.Connection> HelloReceived;
        public event EventHandler<Tuple<Models.Connection, List<Models.ConnectionManifestFeature>>> ManifestReceived;
        public event EventHandler<Models.Connection> ConnectionChanged;

        public Connect()
        {

        }

        public async Task TryConnect(Guid connectionId)
        {
            var model = await connectionStore.GetItemAsync(connectionId);
            await OnTryConnect(model);
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
            foreach (var item in authHubs)
            {
                await item.Value.CloseConnections();
            }
            foreach (var item in featureHubs)
            {
                await item.Value.CloseConnections();
            }
            authHubs.Clear();
            featureHubs.Clear();
        }

        public async Task ValidateKey(Guid connectionId, string key)
        {
            try
            {
                var model = await connectionStore.GetItemAsync(connectionId);
                var con = await OnGetHubConnection(OnGetHub(model, HubTypes.Auth), model);
                if (con != null)
                {
                    logger.Message($"Send key verify to {model.DisplayName} ({DateTime.Now})");
                    await con.InvokeAsync("AuthorizeKey", model.User, key);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public async Task<HubConnection> GetConnection(Models.Connection model, HubTypes hubTypes)
        {
            return await OnGetHubConnection(OnGetHub(model, hubTypes), model);
        }


        private async Task OnTryConnect(Models.Connection model)
        {
            try
            {
                var con = await OnGetHubConnection(OnGetHub(model, HubTypes.Auth), model);
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

        private IInternalHub OnGetHub(Models.Connection model, HubTypes hubTypes)
        {
            if (model == null)
                return null;
            switch (hubTypes)
            {
                case HubTypes.Auth:
                    if (!authHubs.ContainsKey(model.Id))
                    {
                        var authHub = new AuthConnectinHub();
                        authHub.ConnectionChanged += AuthHub_ConnectionChanged;
                        authHub.HelloReceived += HelloReceived;
                        authHub.ManifestReceived += ManifestReceived;
                        authHubs.Add(model.Id, authHub);
                    }
                    return authHubs[model.Id];
                case HubTypes.Feature:
                    if (!featureHubs.ContainsKey(model.Id))
                    {
                        var featureHub = new FeaturesConnectionHub();
                        featureHub.ConnectionChanged += FeatureHub_ConnectionChanged;
                        featureHubs.Add(model.Id, featureHub);
                    }
                    return featureHubs[model.Id];
            }
            return null;
        }

        private void FeatureHub_ConnectionChanged(object sender, Models.Connection e)
        {
            System.Diagnostics.Debug.Print($"FeatureHub_ConnectionChanged {e.LastKnownAddress}" + Environment.NewLine);
        }

        private void AuthHub_ConnectionChanged(object sender, Models.Connection e)
        {
            System.Diagnostics.Debug.Print($"AuthHub_ConnectionChanged {e.LastKnownAddress}" + Environment.NewLine);
        }

        private async Task<HubConnection> OnGetHubConnection(IInternalHub internalHub, Models.Connection model)
        {
            if (internalHub == null)
                return null;
            return await internalHub.GetConnection(model);
        }
    }
}
