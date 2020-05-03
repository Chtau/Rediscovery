using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.RemoteResource
{
    public class DesktopHubRemoteResourceService : IDesktopHubRemoteResourceService
    {
        private readonly SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly HubConnection connection;

        public event EventHandler<List<SharedCoreModels.DeviceInfo>> ActiveDeviceInfoReceived;
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> DeviceInfoReceived;
        public event EventHandler<List<SharedCoreModels.DeviceFeature>> ServiceFeatureReceived;
        public event EventHandler<LoggerEntryModel> LogEntryReceived;

        public DesktopHubRemoteResourceService(IOptions<SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration> remoteResourceSettings)
        {
            _remoteResourceSettings = remoteResourceSettings.Value;
            var baseUrl = "https://" + _remoteResourceSettings.IP;
            if (_remoteResourceSettings.Port != null)
                baseUrl += ":" + _remoteResourceSettings.Port;
            connection = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}/remote/resource/hub")
                .WithAutomaticReconnect()
                .Build();

            /*connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };*/
            connection.Reconnecting += error =>
            {
                Debug.Assert(connection.State == HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.

                return Task.CompletedTask;
            };

            connection.On<List<SharedCoreModels.DeviceInfo>>("ActiveDeviceInfo", (deviceInfos) =>
            {
                ActiveDeviceInfoReceived?.Invoke(this, deviceInfos);
            });
            connection.On<List<SharedCoreModels.DeviceInfo>>("DeviceInfo", (deviceInfos) =>
            {
                DeviceInfoReceived?.Invoke(this, deviceInfos);
            });
            connection.On<List<SharedCoreModels.DeviceFeature>>("ServiceFeature", (deviceInfos) =>
            {
                ServiceFeatureReceived?.Invoke(this, deviceInfos);
            });
            connection.On<LoggerEntryModel>("LogEntry", (entry) =>
            {
                LogEntryReceived?.Invoke(this, entry);
            });
        }

        public async Task Connect()
        {
            if (connection.State != HubConnectionState.Connected)
            {
                await connection.StartAsync();
            }
            await connection.InvokeAsync("Hello", _remoteResourceSettings.DesktopHubApplicationKey);
        }
    }
}
