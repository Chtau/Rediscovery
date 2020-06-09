using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceConsumer.Models
{
    public class FeatureDetail
    {
        public Guid FeatureId { get; set; }
        public PluginFeature.Models.DeviceFeatureSetting Setting { get; set; }
        public List<PluginFeature.Models.DeviceFeatureProfil> Profils { get; set; }
        public byte[] SettingUI { get; set; }
        public byte[] ProfileUI { get; set; }
    }
}
