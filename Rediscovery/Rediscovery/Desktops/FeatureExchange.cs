using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.Features.Authentication.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Desktops.FeatureExchange))]
namespace Rediscovery.Desktops
{
    public class FeatureExchange : IFeatureExchange
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private Dictionary<Guid, HubConnection> connections = new Dictionary<Guid, HubConnection>();

        public event EventHandler<(Guid connectionId, Guid featureId, object data)> DesktopResponseReceived;

        public FeatureExchange()
        {

        }

        public async Task Send(Connection model, ConnectionManifestFeature feature, object data)
        {
            var con = await OnGetConnection(model);
            if (con != null)
            {
                logger.Message($"send feature message to {model.DisplayName} ({DateTime.Now})");
                await con.InvokeAsync("ClientMessage", feature.FeatureId, data);
            }
        }

        private void OnDesktopRespone(HubConnection con, Connection model)
        {
            con.On<Guid, object>("ClientResponse", (Guid featureId, object data) =>
            {
                logger.Message($"Desktop response received from {model.DisplayName} ({DateTime.Now})");
                DesktopResponseReceived?.Invoke(this, (model.Id, featureId, data));
            });
        }

        private async Task<HubConnection> OnGetConnection(Connection model)
        {
            if (model == null)
                return null;

            if (connections.ContainsKey(model.Id))
            {
                if (connections[model.Id].State != HubConnectionState.Connected)
                {
                    logger.Message($"try connect to {model.DisplayName} ({DateTime.Now})");
                    await connections[model.Id].StartAsync();
                }
            }
            else
            {
                var connection = new HubConnectionBuilder()
                .WithUrl("http://" + model.LastKnownAddress + "/hubs/feature", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(model.Token);
                })
                .Build();

                connections.Add(model.Id, connection);
                logger.Message($"try connect to {model.DisplayName} ({DateTime.Now})");
                await connections[model.Id].StartAsync();
            }
            return connections[model.Id];
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
