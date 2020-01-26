using SharedCoreModels.DeviceFeature;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopFeatureMediaPlayer
{
    public class DeviceFeatureMediaPlayer : IDeviceFeatureImplementation
    {
        private DeviceFeatureData currentDeviceFeatureData;

        public event EventHandler<DeviceFeatureData> SendData;

        public DeviceFeatureMediaPlayer()
        {
            
        }

        public void Dispose()
        {

        }

        public DeviceFeature GetDeviceFeatureInfo()
        {
            return new DeviceFeature
            {
                DisplayName = "Media Player",
                Id = new Guid("36CCEE18-583F-4ED9-82E9-3033495665DB"),
                ControlIntegrationPoint = DeviceFeature.IntegrationPoint.Mobile,
                FeatureIntegrationPoint = DeviceFeature.IntegrationPoint.Desktop,
                MinControlIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                Version = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
            };
        }

        public void Init()
        {
            
        }

        public void ReceiveData(DeviceFeatureData data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.Data?.ToString()))
            {
                currentDeviceFeatureData = data;
                if (currentDeviceFeatureData.Data is SharedCoreModels.FeatureModels.VLC.VLCCommandModel commandModel && commandModel != null)
                {
                    //OnHandleCommand(commandModel);
                }
                else
                {
                    System.Diagnostics.Debug.Fail("VLC: Unknown object from Data received");
                }
            }
        }
    }
}
