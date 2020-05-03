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
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO.Compression;
using PluginFeature.Models;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Connection.Connect))]
namespace Rediscovery.Features.Connection
{
    public class Connect : BaseService, IConnect
    {
        internal const string Protocol = "http://";

        public enum HubTypes
        {
            Auth,
            Feature,
        }

        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();
        private IDataStoreGuid<Settings.Models.SettingModel> settingStore => DependencyService.Get<IDataStoreGuid<Settings.Models.SettingModel>>() ?? new Settings.SettingStore();
        private IFeatureExchange featureExchange => DependencyService.Get<IFeatureExchange>() ?? new FeatureExchange();
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();

        private Dictionary<Guid, IInternalHub> authHubs = new Dictionary<Guid, IInternalHub>();
        private Dictionary<Guid, IInternalHub> featureHubs = new Dictionary<Guid, IInternalHub>();
        private HttpClient featureHttpClient = null;

        public event EventHandler<DesktopConfiguration.DesktopConfigurationModel> HelloReceived;
        public event EventHandler<Tuple<DesktopConfiguration.DesktopConfigurationModel, List<Models.ConnectionManifestFeature>>> ManifestReceived;
        public event EventHandler<DesktopConfiguration.DesktopConfigurationModel> ConnectionChanged;

        public Connect()
        {

        }

        public bool IsConnected(DesktopConfiguration.DesktopConfigurationModel model, HubTypes hubType)
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

        public async Task TryConnect(DesktopConfiguration.DesktopConfigurationModel desktopConfigurationModel)
        {
            await OnTryConnect(desktopConfigurationModel);
        }

        public async Task AutoConnect()
        {
            var models = await desktopStore.GetItemsAsync();
            DesktopConfiguration.DesktopConfigurationModel model = null;
            if (models?.Any(x => x.AutoConnect) == true)
            {
                model = models.FirstOrDefault(x => x.AutoConnect);
            }
            if (model != null)
                await OnTryConnect(model);
            return;
        }

        public async Task<HubConnection> GetConnectionAuth(Guid modelId)
        {
            var model = await GetModel(modelId);
            if (model != null)
                return await OnGetHubConnection(await OnGetHub(model, HubTypes.Auth), model);
            return null;
        }

        public async Task<HubConnection> GetConnectionFeature(Guid modelId)
        {
            var model = await GetModel(modelId);
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
                entityManager.Clear(item.Key);
                await item.Value.CloseConnections();
            }
            authHubs.Clear();
            featureHubs.Clear();
        }

        public async Task ValidateKey(Guid connectionId, string key)
        {
            try
            {
                var model = await desktopStore.GetItemAsync(connectionId);
                var con = await OnGetHubConnection(await OnGetHub(model, HubTypes.Auth), model);
                if (con != null)
                {
                    _logger.Message($"Send key verify to {model.DisplayName} ({DateTime.Now})");
                    var setting = await settingStore.GetItemAsync(Guid.Empty);
                    await con.InvokeAsync("AuthorizeKey", setting.DeviceIdentifier, key);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public async Task<DesktopConfiguration.DesktopConfigurationModel> GetModel(Guid id)
        {
            try
            {
                var model = await desktopStore.GetItemAsync(id);
                if (model != null)
                {
                    return model;
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        public async Task<List<DesktopConfiguration.DesktopConfigurationModel>> GetConnectedModels()
        {
            try
            {
                var models = await desktopStore.GetItemsAsync();
                if (models?.Any(x => x.ConnectionState == Enums.ConnectionState.OK) == true)
                {
                    return models.Where(x => x.ConnectionState == Enums.ConnectionState.OK).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        private async Task<HubConnection> GetConnection(DesktopConfiguration.DesktopConfigurationModel model, HubTypes hubTypes)
        {
            return await OnGetHubConnection(await OnGetHub(model, hubTypes), model);
        }


        private async Task OnTryConnect(DesktopConfiguration.DesktopConfigurationModel model)
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
                    _logger.Message($"Could not create connection to {model.DisplayName} ({DateTime.Now})");
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async Task<IInternalHub> OnGetHub(DesktopConfiguration.DesktopConfigurationModel model, HubTypes hubTypes)
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

        private async void AuthHub_HelloReceived(object sender, DesktopConfiguration.DesktopConfigurationModel e)
        {
            try
            {
                HelloReceived?.Invoke(this, e);
                await OnAfterChangedAuthenticationConnection(e);
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }

        private void FeatureHub_ConnectionChanged(object sender, DesktopConfiguration.DesktopConfigurationModel e)
        {
            System.Diagnostics.Debug.Print($"FeatureHub_ConnectionChanged {e.LastKnownAddress}" + Environment.NewLine);
            ConnectionChanged?.Invoke(this, e);
        }

        private void AuthHub_ConnectionChanged(object sender, DesktopConfiguration.DesktopConfigurationModel e)
        {
            System.Diagnostics.Debug.Print($"AuthHub_ConnectionChanged {e.LastKnownAddress}" + Environment.NewLine);
            ConnectionChanged?.Invoke(this, e);
        }

        private async Task<HubConnection> OnGetHubConnection(IInternalHub internalHub, DesktopConfiguration.DesktopConfigurationModel model)
        {
            if (internalHub == null)
                return null;
            return await internalHub.GetConnection(model);
        }

        private async Task OnAfterChangedAuthenticationConnection(DesktopConfiguration.DesktopConfigurationModel model)
        {
            if (featureHubs.ContainsKey(model.Id))
            {
                await featureHubs[model.Id].CloseConnections();
                featureHubs.Remove(model.Id);
            }
            await featureExchange.InitConnectionAsync(model.Id);
        }

        private async Task<HttpClient> GetHttpClientFeature(Guid modelId)
        {
            if (featureHttpClient == null)
            {
                var model = await GetModel(modelId);
                featureHttpClient = new HttpClient();
                featureHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", model.Token);
            }
            return featureHttpClient;
        }

        private async Task<HttpResponseMessage> GetResponseMessage(Guid modelId, Guid featureId, string subUrl)
        {
            try
            {
                var model = await GetModel(modelId);
                var client = await GetHttpClientFeature(modelId);
                var response = await client.GetAsync($"{Protocol}{model.LastKnownAddress}{subUrl}{featureId}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    featureHttpClient.CancelPendingRequests();
                    featureHttpClient.Dispose();
                    featureHttpClient = null;
                    var clientRetry = await GetHttpClientFeature(modelId);
                    return await clientRetry.GetAsync($"{Protocol}{model.LastKnownAddress}{subUrl}{featureId}");
                } else
                {
                    return response;
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
                featureHttpClient.CancelPendingRequests();
                featureHttpClient.Dispose();
                return new HttpResponseMessage(System.Net.HttpStatusCode.ExpectationFailed);
            }
        }

        public async Task<ZipArchive> GetUIArchive(Guid modelId, Guid featureId)
        {
            var response = await GetResponseMessage(modelId, featureId, "/features/ui/");
            if (response.IsSuccessStatusCode)
            {
                var file = await response.Content.ReadAsStreamAsync();
                ZipArchive archive = new ZipArchive(file);
                if (archive != null)
                {
                    return archive;
                }
            }
            return null;
        }

        public async Task<List<DeviceFeatureProfil>> GetDeviceFeatureProfils(Guid modelId, Guid featureId)
        {
            var response = await GetResponseMessage(modelId, featureId, "/features/profiles/");
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeviceFeatureProfil>>(content);
                } catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
            return null;
        }

        public async Task<DeviceFeatureSetting> GetDeviceFeatureSetting(Guid modelId, Guid featureId)
        {
            var response = await GetResponseMessage(modelId, featureId, "/features/settings/");
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceFeatureSetting>(content);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
            return null;
        }
    }
}
