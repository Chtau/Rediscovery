using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Service.DAL.Models
{
    public class DeviceMetadata
    {
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string OSVersion { get; set; }
        public string Platform { get; set; }
        public string Idiom { get; set; }
        public string DeviceType { get; set; }

        public static void UpdateInstance(DeviceMetadata source, DeviceMetadata target)
        {
            target.DeviceType = source.DeviceType;
            target.Idiom = source.Idiom;
            target.Manufacturer = source.Manufacturer;
            target.Model = source.Model;
            target.OSVersion = source.OSVersion;
            target.Platform = source.Platform;
        }
    }
}
