using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public class DeviceInfoMetadata
    {
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string OSVersion { get; set; }
        public string Platform { get; set; }
        public string Idiom { get; set; }
        public string DeviceType { get; set; }

        public void SetMetadata(string model, string manufacturer, string oSVersion,
            string platform, string idiom, string deviceType)
        {
            Model = model;
            Manufacturer = manufacturer;
            OSVersion = oSVersion;
            Platform = platform;
            Idiom = idiom;
            DeviceType = deviceType;
        }
    }
}
