using Rediscovery.Feature.Plugin.Interfaces;
using Rediscovery.Feature.Plugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin
{
    public class BaseClientFeature : BaseFeature<PluginFeatureDataClient, PluginFeatureDefinitionClient>, IClientFeatureImplementation
    {
        public Dictionary<string, string> ActiveDevices = new Dictionary<string, string>();

        public void SetDevices(Dictionary<string, string> devices)
        {
            ActiveDevices.Clear();
            if (devices != null )
                ActiveDevices = devices;
        }
    }
}
