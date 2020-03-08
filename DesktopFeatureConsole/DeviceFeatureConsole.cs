using System;
using System.Collections.Generic;
using System.Text;
using PluginFeature;
using PluginFeature.Models;

namespace DesktopFeatureConsole
{
    public class DeviceFeatureConsole : BaseDeviceFeature
    {
        private readonly Terminal terminal;

        public DeviceFeatureConsole()
        {
            terminal = new Terminal();
            terminal.Output += Terminal_Output;
        }

        private void Terminal_Output(object sender, CommandQueue<string, List<string>> e)
        {
            var data = new DeviceFeatureData
            {
                Data = e.OutgoingData[e.OutgoingData.Count - 1],
                DeviceId = e.DeviceId
            };
            OnSendData(this, data);
        }

        public override DeviceFeatureDefinition GetDeviceFeatureInfo()
        {
            return new DeviceFeatureDefinition
            {
                DisplayName = "Console",
                Id = new Guid("558AC5BD-5B76-433D-8CD7-BCBB7596AAA1"),
                ControlIntegrationPoint = IntegrationPoint.Mobile,
                FeatureIntegrationPoint = IntegrationPoint.Desktop,
                ControlIntegration = ControlIntegrationType.Terminal,
                MinControlIntegrationPoint = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                SettingsObject = null,
                Version = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                Profiles = null
            };
        }

        public override void Dispose()
        {
            base.Dispose();
            terminal.Close();
        }

        public override void ReceiveData(DeviceFeatureData data)
        {
            base.ReceiveData(data);
            if (data != null && IsRegister(data.DeviceId))
            {
                if (!string.IsNullOrWhiteSpace(data.Data?.ToString()))
                {
                    terminal.NewCommand(new CommandQueue<string, List<string>>
                    {
                        IncomingData = data.Data.ToString(),
                        DeviceId = data.DeviceId,
                    });
                }
            }
        }

        public override void Register(string deviceId)
        {
            base.Register(deviceId);
        }

        public override void Unregister(string deviceId)
        {
            base.Unregister(deviceId);
        }
    }
}
