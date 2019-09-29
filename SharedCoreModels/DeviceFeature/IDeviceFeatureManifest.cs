using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public interface IDeviceFeatureManifest
    {
        DeviceFeature GetDeviceFeatureInfo();
    }
}
