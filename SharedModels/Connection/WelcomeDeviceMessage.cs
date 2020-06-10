using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Connection
{
    public class WelcomeDeviceMessage
    {
        public string DeviceIdentifier { get; set; }
        public string DeviceName { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string OSVersion { get; set; }
        public string Platform { get; set; }
        public string Idiom { get; set; }
        public string DeviceType { get; set; }
    }
}
