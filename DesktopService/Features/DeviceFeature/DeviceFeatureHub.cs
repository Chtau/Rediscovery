using DesktopService.Features.RemoteResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.DeviceFeature
{
    [Authorize]
    public class DeviceFeatureHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            _remoteResourcesSenderService.AddActiveDevice(Context.UserIdentifier);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _remoteResourcesSenderService.RemoveActiveDevice(Context.UserIdentifier);
            return base.OnDisconnectedAsync(exception);
        }

        private readonly IFeatureService _featureService;
        private readonly CommunicationResourceProvider.IRemoteResourcesSenderService _remoteResourcesSenderService;

        public DeviceFeatureHub(IFeatureService featureService,
            CommunicationResourceProvider.IRemoteResourcesSenderService remoteResourcesSenderService)
        {
            _featureService = featureService;
            _remoteResourcesSenderService = remoteResourcesSenderService;
        }

        public void ClientMessage(Guid featureId, string profileId, object data)
        {
            System.Diagnostics.Debug.Print($"Feature (id: {featureId}) Message on Service received");
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                var val = new DeviceFeatureData(Context.UserIdentifier, featureId, profileId, data);
                feature.ReceiveData(val);
            }
        }

        public void ClientFeatureStart(Guid featureId)
        {
            System.Diagnostics.Debug.Print($"Feature (id: {featureId}) START on Service received");
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                feature.Register(Context.UserIdentifier);
            }
        }

        public void ClientFeatureStop(Guid featureId)
        {
            System.Diagnostics.Debug.Print($"Feature (id: {featureId}) STOP on Service received");
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                feature.Unregister(Context.UserIdentifier);
            }
        }
    }
}
