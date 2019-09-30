using SharedCoreModels.DeviceFeature;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.DeviceFeature
{
    public interface IFeatureService
    {
        //Tuple<Guid, object>
        event EventHandler<(Guid Id, object Data)> FeatureResponse;
        void Load();
        IDeviceFeatureImplementation GetFeature(Guid featureId);
        List<SharedCoreModels.DeviceFeature.DeviceFeature> GetFeaturesManifest();
    }
}
