using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature
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
