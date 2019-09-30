using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels.DeviceFeature;

namespace DesktopService.Features.DeviceFeature
{
    public class FeatureService : IFeatureService
    {
        public IDeviceFeatureImplementation GetFeature(Guid featureId)
        {
            throw new NotImplementedException();
        }

        public List<SharedCoreModels.DeviceFeature.DeviceFeature> GetFeaturesManifest()
        {
            throw new NotImplementedException();
        }
    }
}
