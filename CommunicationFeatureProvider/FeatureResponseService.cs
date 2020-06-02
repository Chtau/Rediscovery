using Microsoft.AspNetCore.SignalR;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public class FeatureResponseService : IFeatureResponseService
    {
        public FeatureResponseService()
        {
            
        }

        public void ResponseToClient(Guid featureId, DeviceFeatureData data)
        {
            //_hubContext.Clients.User(data.DeviceId).SendAsync("ClientResponse", featureId, data.ProfileId, data.Data);
        }
    }
}
