using Microsoft.AspNetCore.SignalR;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public class FeatureResponseService : IFeatureResponseService
    {
        private readonly IHubContext<FeatureHub> _hubContext;

        public FeatureResponseService(IHubContext<FeatureHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void ResponseToClient(Guid featureId, DeviceFeatureData data)
        {
            _hubContext.Clients.User(data.DeviceId).SendAsync("ClientResponse", featureId, data.ProfileId, data.Data);
        }
    }
}
