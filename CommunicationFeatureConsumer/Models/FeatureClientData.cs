using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureConsumer.Models
{
    public class FeatureClientData
    {
        public Guid FeatureId { get; set; }
        public List<FeatureProfil> FeatureProfils { get; set; }
        public FeatureSetting FeatureSetting { get; set; }
        public byte[] UIArchive { get; set; }
    }
}
