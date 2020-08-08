using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature
{
    public class BaseClientFeature : BaseFeature<PluginFeatureDataClient, PluginFeatureDefinitionClient>, IClientFeatureImplementation
    {
        public List<string> ActiveDevices = new List<string>();

        public void SetDevices(params string[] deviceIds)
        {
            ActiveDevices.Clear();
            ActiveDevices.AddRange(deviceIds);
        }
    }
}
