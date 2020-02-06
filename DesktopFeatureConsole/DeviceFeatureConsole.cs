using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels.DeviceFeature;

namespace DesktopFeatureConsole
{
    public class DeviceFeatureConsole : BaseDeviceFeature
    {
        private readonly Terminal terminal;
        private DeviceFeatureData currentDeviceFeatureData;

        public DeviceFeatureConsole()
        {
            terminal = new Terminal();
            terminal.Output += Terminal_Output;
        }

        private void Terminal_Output(object sender, string e)
        {
            var data = new DeviceFeatureData
            {
                Data = e,
                DeviceId = currentDeviceFeatureData?.DeviceId
            };
            OnSendData(this, data);
        }

        public override DeviceFeature GetDeviceFeatureInfo()
        {
            return new DeviceFeature
            {
                DisplayName = "Console",
                Id = new Guid("558AC5BD-5B76-433D-8CD7-BCBB7596AAA1"),
                ControlIntegrationPoint = DeviceFeature.IntegrationPoint.Mobile,
                FeatureIntegrationPoint = DeviceFeature.IntegrationPoint.Desktop,
                ControlIntegration = DeviceFeature.ControlIntegrationType.Terminal,
                MinControlIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                SettingsObject = null,
                Version = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
            };
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Dispose()
        {
            base.Dispose();
            terminal.Close();
        }

        public override void ReceiveData(DeviceFeatureData data)
        {
            base.ReceiveData(data);
            if (data != null && !string.IsNullOrWhiteSpace(data.Data?.ToString()))
            {
                currentDeviceFeatureData = data;
                terminal.WriteLine(data.Data.ToString());
            }
        }

        public override void Start(string deviceId)
        {
            base.Start(deviceId);
        }

        public override void Stop(string deviceId)
        {
            base.Stop(deviceId);
        }
    }
}
