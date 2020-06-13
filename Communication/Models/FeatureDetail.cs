using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceConsumer.Models
{
    public class FeatureDetail
    {
        public Guid FeatureId { get; set; }
        public FeatureSetting Setting { get; set; }
        public List<FeatureProfil> Profils { get; set; }
        public byte[] SettingUI { get; set; }
        public byte[] ProfileUI { get; set; }
    }
}
