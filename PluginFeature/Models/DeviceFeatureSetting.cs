using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Models
{
    public class DeviceFeatureSetting
    {
        public DeviceFeatureSetting()
        {

        }

        public DeviceFeatureSetting(object data)
        {
            Data = data;
        }

        public object Data { get; set; }
    }
}
