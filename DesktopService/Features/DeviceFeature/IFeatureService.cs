using SharedCoreModels.DeviceFeature;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.DeviceFeature
{
    public interface IFeatureService
    {
        IDeviceFeatureImplementation GetFeature(Guid featureId);
        List<SharedCoreModels.DeviceFeature.DeviceFeature> GetFeaturesManifest();
    }
}
