using CommunicationBase;
using Microsoft.AspNetCore.SignalR.Client;
using PluginFeature.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationClientConsumer
{
    public class Hub : IHub
    {
        private ILogger _logger;
        private IConnectionProvider<HubConnection> _connectionProviderAuthentication;
        private IConnectionProvider<HubConnection> _connectionProvider;

        public event EventHandler<Models.ResponseReceived> FeatureResponseReceived;

        public void Init(ILogger logger, string authHubLink, string exchangeHubLink, Protocol protocol = Protocol.HTTP)
        {
            _logger = logger;
            _connectionProviderAuthentication = new ConnectionProviderSignalR();
            _connectionProvider = new ConnectionProviderSignalR();
            _connectionProviderAuthentication.Init(_logger, authHubLink, protocol);
            _connectionProvider.Init(_logger, exchangeHubLink, protocol);
        }

        public void Authenticate(WelcomeDeviceMessage welcomeDeviceMessage, ConnectionConfiguration configuration, Action<ConnectionConfiguration, bool> callback, Action<Manifest> manifestCallback)
        {
            Disconnect();
            Task.Run(async () =>
            {
                await _connectionProviderAuthentication.Connect(async (result, connection) =>
                {
                    if (result)
                    {
                        try
                        {
                            connection.On<Manifest>("Manifest", (manifest) =>
                            {
                                _logger.Message($"Manifest received for {configuration.DisplayName} ({DateTime.Now})");
                                manifestCallback?.Invoke(manifest);
                            });
                            connection.On<ConnectionState, string>("Hello", (state, token) =>
                            {
                                _logger.Message($"Hello received for {configuration.DisplayName} ({DateTime.Now})");
                                if (!string.IsNullOrWhiteSpace(token))
                                {
                                    configuration.Token = token;
                                    configuration.State = state;
                                    callback.Invoke(configuration, true);
                                }
                                else
                                {
                                    configuration.Token = null;
                                    configuration.State = state;
                                    callback.Invoke(configuration, false);
                                }
                            });
                            await connection.InvokeAsync("Welcome", welcomeDeviceMessage);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                            configuration.Token = null;
                            configuration.State = ConnectionState.Error;
                            callback.Invoke(configuration, false);
                        }
                    }
                    else
                    {
                        configuration.Token = null;
                        configuration.State = ConnectionState.Offline;
                        callback.Invoke(configuration, false);
                    }
                }, configuration, false);
            });
        }

        public void Connect(string deviceIdentifier, ConnectionConfiguration configuration, Action<bool> resultCallback)
        {
            try
            {
                _connectionProvider.CloseConnection();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            Task.Run(async () =>
            {
                await _connectionProvider.Connect((result, connection) =>
                {
                    if (result)
                    {
                        try
                        {
                            connection.On<Guid, string, object>("ClientResponse", (Guid featureId, string profileId, object data) =>
                            {
                                _logger.Message($"Feature response received (ConfigurationId:{configuration.Id} FeatureId:{featureId} ProfileId:{profileId} At:{DateTime.Now})");
                                FeatureResponseReceived?.Invoke(this, new Models.ResponseReceived(configuration.Id, featureId, profileId, data));
                            });
                            resultCallback?.Invoke(true);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                            resultCallback?.Invoke(false);
                        }
                    } else
                    {
                        resultCallback?.Invoke(false);
                    }
                }, configuration, true);
            });
        }

        public void Disconnect()
        {
            try
            {
                _connectionProvider.CloseConnection();
                _connectionProviderAuthentication.CloseConnection();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void Send(Guid featureId, string profileId, object data)
        {
            if (featureId != Guid.Empty)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.Message($"Send feature message (FeatureId:{featureId} ProfileId:{profileId} At:{DateTime.Now})");
                        await _connectionProvider.CurrentConnection.InvokeAsync("ClientMessage", featureId, profileId, data);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                });
            }
        }

        public void Start(Guid featureId)
        {
            if (featureId != Guid.Empty)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.Message($"Start feature usage (FeatureId:{featureId} At:{DateTime.Now})");
                        await _connectionProvider.CurrentConnection.InvokeAsync("ClientFeatureStart", featureId);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                });
            }
        }

        public void Stop(Guid featureId)
        {
            if (featureId != Guid.Empty)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.Message($"Stop feature usage (FeatureId:{featureId} At:{DateTime.Now})");
                        await _connectionProvider.CurrentConnection.InvokeAsync("ClientFeatureStop", featureId);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                });
            }
        }

        public async Task<ZipArchive> GetUIArchive(Guid featureId)
        {
            var response = await GetResponseMessage(featureId, "/features/ui/");
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

        public async Task<List<DeviceFeatureProfil>> GetDeviceFeatureProfils(Guid featureId)
        {
            var response = await GetResponseMessage(featureId, "/features/profiles/");
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeviceFeatureProfil>>(content);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
            return null;
        }

        public async Task<DeviceFeatureSetting> GetDeviceFeatureSetting(Guid featureId)
        {
            var response = await GetResponseMessage(featureId, "/features/settings/");
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

        private async Task<HttpResponseMessage> GetResponseMessage(Guid featureId, string subUrl)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _connectionProvider.Token);
                var response = await client.GetAsync($"{_connectionProvider.BaseUrl}{subUrl}{featureId}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    client.CancelPendingRequests();
                    client.Dispose();
                    client = null;
                    var clientRetry = new HttpClient();
                    clientRetry.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _connectionProvider.Token);
                    return await clientRetry.GetAsync($"{_connectionProvider.BaseUrl}{subUrl}{featureId}");
                }
                else
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new HttpResponseMessage(System.Net.HttpStatusCode.ExpectationFailed);
            }
        }
    }
}
