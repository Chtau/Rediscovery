using PluginFeature;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientFeatureFileExchange
{
    public class ClientFeatureFileExchange : BaseClientFeature
    {
        public override PluginFeatureDefinitionClient GetDeviceFeatureInfo()
        {
            return new PluginFeatureDefinitionClient
            {
                DisplayName = "File exchange",
                Id = new Guid("7C7BE7CA-DE13-4975-A099-C64FA1581E4A"),
                ControlIntegrationPoint = Enums.PluginIntegration.Desktop,
                FeatureIntegrationPoint = Enums.PluginIntegration.Mobile,
                MinimalControlIntegrationPoint = new PluginVersion() { Major = 0, Minor = 0 },
                MinimalFeatureIntegrationPoint = new PluginVersion() { Major = 0, Minor = 0 },
                Version = new PluginVersion() { Major = 0, Minor = 0 },
                Author = "Christoph Taucher",
                Documentation = null,
                Website = null,
                PluginDirectory = PluginDirectory,
                HasProfilConfiguration = false,
                HasSettingConfiguration = false,
                NativeResources = Enums.ClientNativeResources.OpenWithIntent,
                ClientDescription = "Allows to send Files from the Mobile device to a Desktop via the Share functions."
            };
        }

        public override void ReceiveData(PluginExchangeEntity<PluginFeatureDataClient> data)
        {
            base.ReceiveData(data);
            if (data != null && IsRegister(data.Entity.DeviceId))
            {
                if (!string.IsNullOrWhiteSpace(data.Entity.Data?.ToString()))
                {
                    pluginLogger.LogError("TODO: implement handle received [OpenWithIntent]");
                }
            }
        }
    }
}
