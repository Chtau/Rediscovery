using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public interface IFeatureResponseService
    {
        void ResponseToClient(Guid featureId, DeviceFeatureData data);
    }
}
