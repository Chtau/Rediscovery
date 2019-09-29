using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels.DeviceFeature;

namespace DesktopFeatureConsole
{
    public class DeviceFeatureConsole : IDeviceFeatureManifest, IDeviceFeatureImplementation<object>
    {
        public event EventHandler<object> SendData;

        public DeviceFeature GetDeviceFeatureInfo()
        {
            return new DeviceFeature
            {
                DisplayName = "Console",
                Id = new Guid("558AC5BD-5B76-433D-8CD7-BCBB7596AAA1"),
                ControlIntegrationPoint = DeviceFeature.IntegrationPoint.Mobile,
                FeatureIntegrationPoint = DeviceFeature.IntegrationPoint.Desktop,
                MinControlIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                Version = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
            };
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void ReceiveData(object data)
        {
            throw new NotImplementedException();
        }
    }
}
