using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SharedCoreModels.DeviceFeature;
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

        public void ClientMessage(Guid featureId, object data)
        {
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                var val = new DeviceFeatureData
                {
                    Data = data,
                    DeviceId = Context.UserIdentifier
                };
                feature.ReceiveData(val);
            }
        }
    }
}
