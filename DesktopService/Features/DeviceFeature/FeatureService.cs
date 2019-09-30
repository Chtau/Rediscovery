using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels.DeviceFeature;
using System.Linq;

namespace DesktopService.Features.DeviceFeature
{
    public class FeatureService : IFeatureService
    {
        private List<IDeviceFeatureImplementation> deviceFeatureImplementations = new List<IDeviceFeatureImplementation>();

        public FeatureService()
        {
            Load();
        }

        public IDeviceFeatureImplementation GetFeature(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId);
        }

        public List<SharedCoreModels.DeviceFeature.DeviceFeature> GetFeaturesManifest()
        {
            var manifest = new List<SharedCoreModels.DeviceFeature.DeviceFeature>();
            foreach (var item in deviceFeatureImplementations)
            {
                manifest.Add(item.GetDeviceFeatureInfo());
            }
            return manifest;
        }

        public void Load()
        {
            deviceFeatureImplementations.Add(new DesktopFeatureConsole.DeviceFeatureConsole());
        }
    }
}
