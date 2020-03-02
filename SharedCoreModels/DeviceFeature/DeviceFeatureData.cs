using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public class DeviceFeatureData
    {
        public string DeviceId { get; set; }
        public string ProfileId { get; set; }
        public object Data { get; set; }
    }
}
