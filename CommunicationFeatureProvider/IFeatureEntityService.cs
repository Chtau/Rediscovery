using PluginFeature.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public interface IFeatureEntityService
    {
        IDeviceFeatureImplementation GetFeature(Guid featureId);
    }
}
