using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels.DeviceFeature;

namespace DesktopFeatureConsole
{
    public class DeviceFeatureConsole : IDeviceFeatureImplementation
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
            SendData?.Invoke(this, data);
        }

        public event EventHandler<DeviceFeatureData> SendData;

        public DeviceFeature GetDeviceFeatureInfo()
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

        public void Init()
        {
            
        }

        public void Dispose()
        {
            terminal.Close();
        }

        public void ReceiveData(DeviceFeatureData data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.Data?.ToString()))
            {
                currentDeviceFeatureData = data;
                terminal.WriteLine(data.Data.ToString());
            }
        }
    }
}
