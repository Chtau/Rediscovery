using System;
using System.Collections.Generic;
using System.Text;
using Rediscovery.Feature.Desktop.Console.Models;
using PluginFeature;
using PluginFeature.Interfaces;
using PluginFeature.Models;

namespace Rediscovery.Feature.Desktop.Console
{
    public class DeviceFeatureConsole : BaseDeviceFeature
    {
        private Terminal terminal;

        public DeviceFeatureConsole()
        {
            
        }

        public override void Init(string pluginDirectory, IPluginLogger pluginLogger)
        {
            base.Init(pluginDirectory, pluginLogger);
            terminal = new Terminal(pluginLogger);
            terminal.Output += Terminal_Output;
        }

        private void Terminal_Output(object sender, CommandQueue<string, List<TerminalData>> e)
        {
            var model = e.OutgoingData[e.OutgoingData.Count - 1];
            var data = new PluginFeatureData(e.DeviceId, GetDeviceFeatureInfo().Id, null, Newtonsoft.Json.JsonConvert.SerializeObject(model));
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
                DisplayName = "Terminal",
                Id = new Guid("558AC5BD-5B76-433D-8CD7-BCBB7596AAA1"),
                ControlIntegrationPoint = Enums.PluginIntegration.Mobile,
                FeatureIntegrationPoint = Enums.PluginIntegration.Desktop,
                MinimalControlIntegrationPoint = new PluginVersion() { Major = 0, Minor = 0 },
                MinimalFeatureIntegrationPoint = new PluginVersion() { Major = 0, Minor = 0 },
                Version = new PluginVersion() { Major = 0, Minor = 0 },
                Author = "Christoph Taucher",
                Documentation = null,
                Website = null,
                PluginDirectory = PluginDirectory,
                HasProfilConfiguration = false,
                HasSettingConfiguration = false
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
                    var commandModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.CommandModel>(data.Entity.Data?.ToString());
                    if (commandModel != null)
                    {
                        terminal.NewCommand(new CommandQueue<string, List<TerminalData>>
                        {
                            IncomingData = commandModel.Input,
                            DeviceId = data.Entity.DeviceId,
                        });
                    }
                    else
                    {
                        pluginLogger?.LogCritical("Terminal: Unknown object from Data received");
                    }
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
