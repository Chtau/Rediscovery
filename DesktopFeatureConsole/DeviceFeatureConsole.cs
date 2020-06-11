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
            var data = new DeviceFeatureData(e.DeviceId, GetDeviceFeatureInfo().Id, null, e.OutgoingData[e.OutgoingData.Count - 1]);
            OnSendData(this, new ExchangeEntity<DeviceFeatureData>
            {
                Sid = e.DeviceId,
                Entity = data
            });
        }

        public override SharedBase.Device.FeatureDefinitionExtended GetDeviceFeatureInfo()
        {
            return new SharedBase.Device.FeatureDefinitionExtended
            {
                DisplayName = "Console",
                Id = new Guid("558AC5BD-5B76-433D-8CD7-BCBB7596AAA1"),
                ControlIntegrationPoint = SharedBase.Device.IntegrationPoint.Mobile,
                FeatureIntegrationPoint = SharedBase.Device.IntegrationPoint.Desktop,
                MinimalControlIntegrationPoint = new SharedBase.Core.Version() { Major = 0, Minor = 0 },
                MinimalFeatureIntegrationPoint = new SharedBase.Core.Version() { Major = 0, Minor = 0 },
                Version = new SharedBase.Core.Version() { Major = 0, Minor = 0 },
                Author = "Christoph Taucher",
                Documentation = null,
                HasProfiles = false,
                ProfileUIReadonly = false,
                SettingUIReadonly = false,
                HasSettings = false,
                Website = null,
                PluginDirectory = PluginDirectory,
                ProfileUIElementName = null,
                SettingUIElementName = null
            };
        }

        public override void Dispose()
        {
            base.Dispose();
            terminal.Close();
        }

        public override void ReceiveData(ExchangeEntity<DeviceFeatureData> data)
        {
            base.ReceiveData(data);
            if (data != null && IsRegister(data.Entity.DeviceId))
            {
                if (!string.IsNullOrWhiteSpace(data.Entity.Data?.ToString()))
                {
                    terminal.NewCommand(new CommandQueue<string, List<string>>
                    {
                        IncomingData = data.Entity.Data.ToString(),
                        DeviceId = data.Entity.DeviceId,
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

        public override DeviceFeatureSetting GetSettingsObject()
        {
            return null;
        }

        public override List<DeviceFeatureProfil> GetProfiles()
        {
            return null;
        }
    }
}
