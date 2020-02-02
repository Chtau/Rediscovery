using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.Features.Authentication.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.FeatureExchange))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class FeatureExchange : IFeatureExchange
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private Features.Authentication.IConnect connection => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();
        private Connection model;
        private HubConnection featureHub;

        public event EventHandler<(Guid connectionId, Guid featureId, object data)> DesktopResponseReceived;

        public FeatureExchange()
        {
            connection.ConnectionChanged += Connection_ConnectionChanged;
            Init().GetAwaiter();
        }

        private void Connection_ConnectionChanged(object sender, Connection e)
        {
            /*if (model != null && e.Id == model.Id)
            {
                Init().GetAwaiter();
            }*/
        }

        private async Task Init()
        {
            this.model = await connection.GetModel();
            if (model != null)
            {
                if (featureHub != null)
                    await connection.CloseConnections();
                featureHub = await connection.GetConnectionFeature();
                if (featureHub != null)
                {
                    featureHub.Remove("ClientResponse");
                    featureHub.On<Guid, object>("ClientResponse", (Guid featureId, object data) =>
                    {
                        logger.Message($"Desktop response received ({DateTime.Now})");
                        DesktopResponseReceived?.Invoke(this, (model.Id, featureId, data));
                    });
                }
            } else
            {
                logger.Message("Feature exchange init without a connection model");
            }
        }

        public async Task Send(ConnectionManifestFeature feature, object data)
        {
            if (featureHub != null)
            {
                logger.Message($"send feature message to {model.DisplayName} ({DateTime.Now})");
                await featureHub.InvokeAsync("ClientMessage", feature.FeatureId, data);
            } else
            {
                logger.Message("Try to send feature exchange message without hub connection");
            }
        }
    }
}
