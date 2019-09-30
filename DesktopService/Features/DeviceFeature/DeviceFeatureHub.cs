using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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

        public DeviceFeatureHub(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        public async Task ClientMessage(Guid featureId, object data)
        {
            
        }

        private async Task ResponseToClient(Guid featureId, object data)
        {
            await Clients.Caller.SendAsync("ClientResponse", featureId, data);
        }
    }
}
