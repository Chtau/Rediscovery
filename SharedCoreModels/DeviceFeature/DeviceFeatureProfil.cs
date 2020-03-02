using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public class DeviceFeatureProfil
    {
        public DeviceFeatureProfil(string id, string displayName, object data)
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
