using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.Services;
using SharedCoreModels;
using Xamarin.Forms;
using System.Linq;
using Rediscovery.Features.DesktopFeatures;
using Rediscovery.Features.Authentication;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Connection.Connect))]
namespace Rediscovery.Features.Connection
{
    public class Connect : IConnect
    {
        public enum HubTypes
        {
            Auth,
            Feature,
        }

        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDataStoreGuid<Models.ConnectionInfo> connectionStore => DependencyService.Get<IDataStoreGuid<Models.ConnectionInfo>>() ?? new ConnectionStore();
        private IFeatureExchange featureExchange => DependencyService.Get<IFeatureExchange>() ?? new FeatureExchange();

        private Dictionary<Guid, IInternalHub> authHubs = new Dictionary<Guid, IInternalHub>();
        private Dictionary<Guid, IInternalHub> featureHubs = new Dictionary<Guid, IInternalHub>();

        public event EventHandler<Models.ConnectionInfo> HelloReceived;
        public event EventHandler<Tuple<Models.ConnectionInfo, List<Models.ConnectionManifestFeature>>> ManifestReceived;
        public event EventHandler<Models.ConnectionInfo> ConnectionChanged;

        public Connect()
        {

        }

        public bool IsConnected(Models.ConnectionInfo model, HubTypes hubType)
        {
            if (model == null)
                return false;
            switch (hubType)
            {
                case HubTypes.Auth:
                    if (authHubs.ContainsKey(model.Id))
                    {
                        return authHubs[model.Id].IsConnected;
                    }
                    break;
                case HubTypes.Feature:
                    if (featureHubs.ContainsKey(model.Id))
                    {
                        return featureHubs[model.Id].IsConnected;
                    }
                    break;
            }
            return false;
        }

        public async Task TryConnect(Guid connectionId)
        {
            var model = await connectionStore.GetItemAsync(connectionId);
            await OnTryConnect(model);
        }

        public async Task AutoConnect()
        {
            var model = await GetModel();
            if (model != null)
                await OnTryConnect(model);
            return;
        }

        public async Task<HubConnection> GetConnectionAuth()
        {
            var model = await GetModel();
            if (model != null)
                return await OnGetHubConnection(await OnGetHub(model, HubTypes.Auth), model);
            return null;
        }

        public async Task<HubConnection> GetConnectionFeature()
        {
            var model = await GetModel();
            return await OnGetHubConnection(await OnGetHub(model, HubTypes.Feature), model);
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
                var con = await OnGetHubConnection(await OnGetHub(model, HubTypes.Auth), model);
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

        public async Task<Models.ConnectionInfo> GetModel()
        {
            try
            {
                var models = await connectionStore.GetItemsAsync();
                var activeModel = models?.FirstOrDefault(x => x.Active);
                if (activeModel != null)
                    return activeModel;
                if (models.Count() == 1)
                {
                    var newActiveModel = models.First();
                    newActiveModel.Active = true;
                    newActiveModel.AutoConnect = true;
                    connectionStore.UpdateItem(newActiveModel);
                    return newActiveModel;
                }
            } catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        private async Task<HubConnection> GetConnection(Models.ConnectionInfo model, HubTypes hubTypes)
        {
            return await OnGetHubConnection(await OnGetHub(model, hubTypes), model);
        }


        private async Task OnTryConnect(Models.ConnectionInfo model)
        {
            try
            {
                var con = await OnGetHubConnection(await OnGetHub(model, HubTypes.Auth), model);
                if (con != null && con.State == HubConnectionState.Connected)
                {
                    // we send the Welcome direct in the AuthConnectionHub
                    //logger.Message($"send welcome to {model.DisplayName} ({DateTime.Now})");
                    //await con.InvokeAsync("Welcome", model.User);
                    //await OnAfterChangedAuthenticationConnection(model);
                } else
                {
                    logger.Message($"Could not create connection to {model.DisplayName} ({DateTime.Now})");
                }
            } catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private async Task<IInternalHub> OnGetHub(Models.ConnectionInfo model, HubTypes hubTypes)
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
                        authHub.HelloReceived += AuthHub_HelloReceived;
                        authHub.ManifestReceived += ManifestReceived;
                        authHubs.Add(model.Id, authHub);
                        //await OnAfterChangedAuthenticationConnection(model);
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

        private async void AuthHub_HelloReceived(object sender, Models.ConnectionInfo e)
        {
            HelloReceived?.Invoke(this, e);
            await OnAfterChangedAuthenticationConnection(e);
        }

        private void FeatureHub_ConnectionChanged(object sender, Models.ConnectionInfo e)
        {
            System.Diagnostics.Debug.Print($"FeatureHub_ConnectionChanged {e.LastKnownAddress}" + Environment.NewLine);
            ConnectionChanged?.Invoke(this, e);
        }

        private void AuthHub_ConnectionChanged(object sender, Models.ConnectionInfo e)
        {
            System.Diagnostics.Debug.Print($"AuthHub_ConnectionChanged {e.LastKnownAddress}" + Environment.NewLine);
            ConnectionChanged?.Invoke(this, e);
        }

        private async Task<HubConnection> OnGetHubConnection(IInternalHub internalHub, Models.ConnectionInfo model)
        {
            if (internalHub == null)
                return null;
            return await internalHub.GetConnection(model);
        }

        private async Task OnAfterChangedAuthenticationConnection(Models.ConnectionInfo model)
        {
            if (featureHubs.ContainsKey(model.Id))
            {
                await featureHubs[model.Id].CloseConnections();
                featureHubs.Remove(model.Id);
            }
            await featureExchange.InitConnectionAsync();
        }
    }
}
