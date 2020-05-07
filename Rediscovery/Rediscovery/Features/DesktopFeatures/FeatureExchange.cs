using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.Features.Authentication.Models;
using Rediscovery.Features.Connection;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.FeatureExchange))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class FeatureExchange : BaseService, IFeatureExchange
    {
        private IConnect connection => DependencyService.Get<IConnect>() ?? new Connect();
        private DesktopConfiguration.DesktopConfigurationModel model;
        private HubConnection featureHub;

        public event EventHandler<(Guid connectionId, Guid featureId, string profileId, object data)> DesktopResponseReceived;

        public FeatureExchange()
        {
            connection.ConnectionChanged += Connection_ConnectionChanged;
        }

        public void InitConnection(Guid modelId)
        {
            InitConnectionAsync(modelId).GetAwaiter();
        }

        public async Task InitConnectionAsync(Guid modelId)
        {
            await Init(modelId);
        }

        private void Connection_ConnectionChanged(object sender, DesktopConfiguration.DesktopConfigurationModel e)
        {
            /*if (model != null && e.Id == model.Id)
            {
                Init().GetAwaiter();
            }*/
        }

        private async Task Init(Guid modelId)
        {
            this.model = await connection.GetModel(modelId);
            if (model != null)
            {
                if (featureHub != null)
                    await connection.CloseConnections();
                featureHub = await connection.GetConnectionFeature(modelId);
                if (featureHub != null)
                {
                    featureHub.Remove("ClientResponse");
                    featureHub.On<Guid, string, object>("ClientResponse", (Guid featureId, string profileId, object data) =>
                    {
                        _logger.Message($"Desktop response received ({DateTime.Now})");
                        DesktopResponseReceived?.Invoke(this, (model.Id, featureId, profileId, data));
                    });
                }
            } else
            {
                _logger.Message("Feature exchange init without a connection model");
            }
        }

        public async Task Send(Connection.Models.ConnectionManifestFeature feature, string profileId, object data)
        {
            if (featureHub != null)
            {
                _logger.Message($"send feature message to {model.DisplayName} ({DateTime.Now})");
                await featureHub.InvokeAsync("ClientMessage", feature.FeatureId, profileId, data);
            } else
            {
                _logger.Message("Try to send feature exchange message without hub connection");
            }
        }

        public async Task Start(Connection.Models.ConnectionManifestFeature feature)
        {
            if (featureHub != null)
            {
                _logger.Message($"Start feature {model.DisplayName} ({DateTime.Now})");
                await featureHub.InvokeAsync("ClientFeatureStart", feature.FeatureId);
            }
            else
            {
                _logger.Message("Try to START feature exchange message without hub connection");
            }
        }

        public async Task Stop(Connection.Models.ConnectionManifestFeature feature)
        {
            if (featureHub != null)
            {
                _logger.Message($"Stop feature {model.DisplayName} ({DateTime.Now})");
                await featureHub.InvokeAsync("ClientFeatureStop", feature.FeatureId);
            }
            else
            {
                _logger.Message("Try to STOP feature exchange message without hub connection");
            }
        }
    }
}
