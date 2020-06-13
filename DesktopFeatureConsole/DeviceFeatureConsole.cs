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
            var data = new PluginFeatureData(e.DeviceId, GetDeviceFeatureInfo().Id, null, e.OutgoingData[e.OutgoingData.Count - 1]);
            OnSendData(this, new PluginExchangeEntity<PluginFeatureData>
            {
                Sid = e.DeviceId,
                Entity = data
            });
        }

        public override PluginFeatureDefinition GetDeviceFeatureInfo()
        {
            return new PluginFeatureDefinition
            {
                DisplayName = "Console",
                Id = new Guid("558AC5BD-5B76-433D-8CD7-BCBB7596AAA1"),
                ControlIntegrationPoint = PluginFeatureDefinition.PluginIntegration.Mobile,
                FeatureIntegrationPoint = PluginFeatureDefinition.PluginIntegration.Desktop,
                MinimalControlIntegrationPoint = new PluginVersion() { Major = 0, Minor = 0 },
                MinimalFeatureIntegrationPoint = new PluginVersion() { Major = 0, Minor = 0 },
                Version = new PluginVersion() { Major = 0, Minor = 0 },
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

        public override void ReceiveData(PluginExchangeEntity<PluginFeatureData> data)
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

        public override PluginFeatureSetting GetSettingsObject()
        {
            return null;
        }

        public override List<PluginFeatureProfil> GetProfiles()
        {
            return null;
        }
    }
}
