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
        private SharedBase.Logging.ILogger _logger;
        private IConnectionProvider<HubConnection> _connectionProviderAuthentication;
        private IConnectionProvider<HubConnection> _connectionProvider;

        public event EventHandler<Models.ResponseReceived> FeatureResponseReceived;

        public void Init(SharedBase.Logging.ILogger logger, string authHubLink, string exchangeHubLink, Protocol protocol = Protocol.HTTP)
        {
            _logger = logger;
            _connectionProviderAuthentication = new ConnectionProviderSignalR();
            _connectionProvider = new ConnectionProviderSignalR();
            _connectionProviderAuthentication.Init(_logger, authHubLink, protocol);
            _connectionProvider.Init(_logger, exchangeHubLink, protocol);
        }

        public void Authenticate(WelcomeDeviceMessage welcomeDeviceMessage, ConnectionConfiguration configuration, Action<ConnectionConfiguration, bool> callback, Action<Manifest> manifestCallback)
        {
            Task.Run(async () =>
            {
                await Disconnect();
                await _connectionProviderAuthentication.Connect(async (result, connection) =>
                {
                    if (result)
                    {
                        try
                        {
                            connection.On<Manifest>("Manifest", (manifest) =>
                            {
                                _logger.LogTrace($"Manifest received for {configuration.DisplayName} ({DateTime.Now})");
                                manifestCallback?.Invoke(manifest);
                            });
                            connection.On<ConnectionState, string>("Hello", (state, token) =>
                            {
                                _logger.LogTrace($"Hello received for {configuration.DisplayName} ({DateTime.Now})");
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
                            _logger.LogError(ex, "Authentication flow failed after a valid authentication");
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

        public void Connect(ConnectionConfiguration configuration, Action<bool, ConnectionState> resultCallback)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _connectionProvider.CloseConnection();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while trying to close the connection before creating a new connection");
                }
                await _connectionProvider.Connect((result, connection) =>
                {
                    if (result)
                    {
                        try
                        {
                            connection.On<Guid, string, object>("ClientResponse", (Guid featureId, string profileId, object data) =>
                            {
                                _logger.LogTrace($"Feature response received (ConfigurationId:{configuration.Id} FeatureId:{featureId} ProfileId:{profileId} At:{DateTime.Now})");
                                FeatureResponseReceived?.Invoke(this, new Models.ResponseReceived(configuration.Id, featureId, profileId, data));
                            });
                            resultCallback?.Invoke(true, ConnectionState.OK);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Connect flow failed after a successful connection was established");
                            resultCallback?.Invoke(false, ConnectionState.Error);
                        }
                    } else
                    {
                        resultCallback?.Invoke(false, ConnectionState.Denied);
                    }
                }, configuration, true);
            });
        }

        public async Task<bool> Disconnect()
        {
            try
            {
                if (_connectionProvider != null)
                    await _connectionProvider.CloseConnection();
                if (_connectionProviderAuthentication != null)
                    await _connectionProviderAuthentication.CloseConnection();
                _logger.LogTrace($"After Disconnect (At:{DateTime.Now})");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection providers throw an error on disconnecting");
                return false;
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
                        _logger.LogTrace($"Send feature message (FeatureId:{featureId} ProfileId:{profileId} At:{DateTime.Now})");
                        await _connectionProvider.CurrentConnection.InvokeAsync("ClientMessage", featureId, profileId, data);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send a [ClientMessage] from the Feature Id:{featureId}");
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
                        _logger.LogTrace($"Start feature usage (FeatureId:{featureId} At:{DateTime.Now})");
                        await _connectionProvider.CurrentConnection.InvokeAsync("ClientFeatureStart", featureId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send the command [ClientFeatureStart] for the Feature Id:{featureId}");
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
                        _logger.LogTrace($"Stop feature usage (FeatureId:{featureId} At:{DateTime.Now})");
                        await _connectionProvider.CurrentConnection.InvokeAsync("ClientFeatureStop", featureId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send the command [ClientFeatureStop] for the Feature Id:{featureId}");
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
                string content = null;
                try
                {
                    content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeviceFeatureProfil>>(content);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Source for deserialization to type [List<DeviceFeatureProfil>] is not valid (Feature id:{featureId})");
                    _logger.LogTrace($"Feature (Id: {featureId}) Profile string value:" + content);
                }
            }
            return null;
        }

        public async Task<DeviceFeatureSetting> GetDeviceFeatureSetting(Guid featureId)
        {
            var response = await GetResponseMessage(featureId, "/features/settings/");
            if (response.IsSuccessStatusCode)
            {
                string content = null;
                try
                {
                    content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceFeatureSetting>(content);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Source for deserialization to type [DeviceFeatureSetting] is not valid (Feature id:{featureId})");
                    _logger.LogTrace($"Feature (Id: {featureId}) Setting string value:" + content);
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
                _logger.LogError(ex, $"HttpClient Feature (Id:{featureId}) failed to get response message on Url:{subUrl}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.ExpectationFailed);
            }
        }

        public void LogEntry(SharedBase.Logging.LoggerEntry e)
        {
            if (e != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _connectionProvider.CurrentConnection.InvokeAsync("LogEntry", e);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                });
            }
        }
    }
}
