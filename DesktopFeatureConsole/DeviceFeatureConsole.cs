using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels.DeviceFeature;

namespace DesktopFeatureConsole
{
    public class DeviceFeatureConsole : IDeviceFeatureManifest, IDeviceFeatureImplementation
    {
        private readonly Terminal terminal;

        public DeviceFeatureConsole()
        {
            terminal = new Terminal();
            terminal.Output += Terminal_Output;
        }

        private void Terminal_Output(object sender, string e)
        {
            SendData?.Invoke(this, e);
        }

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
            
        }

        public void Dispose()
        {
            terminal.Close();
        }

        public void ReceiveData(object data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.ToString()))
            {
                terminal.WriteLine(data.ToString());
            }
        }
    }
}
