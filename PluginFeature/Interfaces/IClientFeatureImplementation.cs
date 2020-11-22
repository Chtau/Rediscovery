using Rediscovery.Feature.Plugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Interfaces
{
    public interface IClientFeatureImplementation : IBaseFeatureImplementation<PluginFeatureDataClient, PluginFeatureDefinitionClient>
    {
        void SetDevices(Dictionary<string, string> devices);
    }
}
