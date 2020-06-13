using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureConsumer.Models
{
    public class FeatureClientData
    {
        public Guid FeatureId { get; set; }
        public List<PluginFeature.Models.PluginFeatureProfil> FeatureProfils { get; set; }
        public PluginFeature.Models.PluginFeatureSetting FeatureSetting { get; set; }
        public byte[] UIArchive { get; set; }
    }
}
