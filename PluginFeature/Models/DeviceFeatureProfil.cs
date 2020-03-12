using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Models
{
    public class DeviceFeatureProfil
    {
        public DeviceFeatureProfil()
        {

        }

        public DeviceFeatureProfil(string id, string displayName, object data) : this()
        {
            Id = id;
            DisplayName = displayName;
            ProfileData = data;
        }

        public string Id { get; set; }
        public string DisplayName { get; set; }
        public object ProfileData { get; set; }
    }
}
