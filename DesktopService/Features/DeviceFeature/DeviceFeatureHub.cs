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
        private readonly IFeatureService _featureService;
        private readonly IHubContext<DeviceFeatureHub> _hubContext;

        public DeviceFeatureHub(IFeatureService featureService, IHubContext<DeviceFeatureHub> hubContext)
        {
            _hubContext = hubContext;
            _featureService = featureService;
        }

        public void ClientMessage(Guid featureId, string profileId, object data)
        {
            System.Diagnostics.Debug.Print($"Feature (id: {featureId}) Message on Service received");
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                var val = new DeviceFeatureData
                {
                    Data = data,
                    DeviceId = Context.UserIdentifier,
                    ProfileId = profileId
                };
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
