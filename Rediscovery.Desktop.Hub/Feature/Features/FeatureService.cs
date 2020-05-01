using IPCPipe.Models;
using Microsoft.AspNetCore.SignalR.Client;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Features
{
    public class FeatureService : IFeatureService
    {
        private readonly HubConnection connection;

        public List<DeviceFeature> Items { get; set; } = new List<DeviceFeature>();

        public event EventHandler<List<DeviceFeature>> DeviceFeatureReceived;

        public FeatureService()
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

            /*connection.On<List<SharedCoreModels.DeviceInfo>>("ActiveDeviceInfo", (deviceInfos) =>
            {
                
            });
            connection.On<List<SharedCoreModels.DeviceInfo>>("DeviceInfo", (deviceInfos) =>
            {

            });*/
            connection.On<List<SharedCoreModels.DeviceFeature>>("ServiceFeature", (deviceInfos) =>
            {
                Items.Clear();
                Items.AddRange(deviceInfos);
                DeviceFeatureReceived?.Invoke(this, deviceInfos);
            });
        }

        public void Init()
        {
            connection.StartAsync();
        }
    }
}
