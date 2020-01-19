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
        private Features.Authentication.IConnect connection => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();
        private Connection model;

        public event EventHandler<(Guid connectionId, Guid featureId, object data)> DesktopResponseReceived;

        public FeatureExchange()
        {
            connection.ConnectionChanged += Connection_ConnectionChanged;
        }

        private void Connection_ConnectionChanged(object sender, Connection e)
        {
            if (model != null && e.Id == model.Id)
            {
                Init(e).GetAwaiter();
            }
        }

        public async Task Init(Connection model)
        {
            this.model = model;
            var con = await connection.GetConnection(this.model, Features.Authentication.Connect.HubTypes.Feature);
            if (con != null)
            {
                OnDesktopRespone(con, this.model);
            }
        }

        public async Task Send(ConnectionManifestFeature feature, object data)
        {
            var con = await connection.GetConnection(this.model, Features.Authentication.Connect.HubTypes.Feature);
            if (con != null)
            {
                logger.Message($"send feature message to {model.DisplayName} ({DateTime.Now})");
                await con.InvokeAsync("ClientMessage", feature.FeatureId, data);
            }
        }

        private void OnDesktopRespone(HubConnection con, Connection model)
        {
            con.Remove("ClientResponse");
            con.On<Guid, object>("ClientResponse", (Guid featureId, object data) =>
            {
                logger.Message($"Desktop response received from {model.DisplayName} ({DateTime.Now})");
                DesktopResponseReceived?.Invoke(this, (model.Id, featureId, data));
            });
        }
    }
}
