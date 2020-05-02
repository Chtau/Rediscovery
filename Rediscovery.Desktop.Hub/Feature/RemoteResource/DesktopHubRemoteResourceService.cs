using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.RemoteResource
{
    public class DesktopHubRemoteResourceService : IDesktopHubRemoteResourceService
    {
        private readonly HubConnection connection;

        public event EventHandler<List<SharedCoreModels.DeviceInfo>> ActiveDeviceInfoReceived;
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> DeviceInfoReceived;
        public event EventHandler<List<SharedCoreModels.DeviceFeature>> ServiceFeatureReceived;

        public DesktopHubRemoteResourceService()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/ChatHub")
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
        }

        public async Task Connect()
        {
            await connection.StartAsync();
            await connection.InvokeAsync("Hello", "hub");
        }
    }
}
