using DesktopService.Features.Pipes;
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
            if (!ActiveUserHandler.UserIds.Contains(Context.UserIdentifier))
                ActiveUserHandler.UserIds.Add(Context.UserIdentifier);
            _pipeRepository.ActiveDeviceInfoChanged();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (ActiveUserHandler.UserIds.Contains(Context.UserIdentifier))
                ActiveUserHandler.UserIds.Remove(Context.UserIdentifier);
            _pipeRepository.ActiveDeviceInfoChanged();
            return base.OnDisconnectedAsync(exception);
        }

        private readonly IFeatureService _featureService;
        private readonly IHubContext<DeviceFeatureHub> _hubContext;
        private readonly IPipeRepository _pipeRepository;

        public DeviceFeatureHub(IFeatureService featureService, IHubContext<DeviceFeatureHub> hubContext,
            IPipeRepository pipeRepository)
        {
            _hubContext = hubContext;
            _featureService = featureService;
            _pipeRepository = pipeRepository;
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
