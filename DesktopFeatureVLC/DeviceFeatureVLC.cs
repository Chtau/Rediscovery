using SharedCoreModels.DeviceFeature;
using System;

namespace DesktopFeatureVLC
{
    public class DeviceFeatureVLC : IDeviceFeatureImplementation
    {
        // TODO: we should send key combinations to programs instead build a specific library

        private DeviceFeatureData currentDeviceFeatureData;
        private VLC vLC;
        private VLCRemote VLCRemote;

        public event EventHandler<DeviceFeatureData> SendData;

        public DeviceFeatureVLC()
        {
            //vLC = new VLC();
            VLCRemote = new VLCRemote();
        }

        public void Dispose()
        {
            
        }

        public DeviceFeature GetDeviceFeatureInfo()
        {
            return new DeviceFeature
            {
                DisplayName = "VLC",
                Id = new Guid("5A3E794B-4CE9-47AB-B7E6-D96FF428CC68"),
                ControlIntegrationPoint = DeviceFeature.IntegrationPoint.Mobile,
                FeatureIntegrationPoint = DeviceFeature.IntegrationPoint.Desktop,
                MinControlIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                Version = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
            };
        }

        public void Init()
        {
            VLCRemote.Play();
            //vLC.Start();
        }

        public void ReceiveData(DeviceFeatureData data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.Data?.ToString()))
            {
                currentDeviceFeatureData = data;
            }
        }
    }
}
