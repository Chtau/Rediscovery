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
            _featureService.FeatureResponse += _featureService_FeatureResponse;
        }

        private void _featureService_FeatureResponse(object sender, (Guid Id, object Data) e)
        {
            ResponseToClient(e.Id, e.Data);
        }

        public void ClientMessage(Guid featureId, object data)
        {
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                feature.ReceiveData(data);
            }
        }

        private void ResponseToClient(Guid featureId, object data)
        {
            Clients.Caller.SendAsync("ClientResponse", featureId, data);
        }
    }
}
