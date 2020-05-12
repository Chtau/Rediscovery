using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public enum DeviceType
    {
        //
        // Summary:
        //     An unknown device type.
        Unknown = 0,
        //
        // Summary:
        //     The device is a physical device, such as an iPhone, Android tablet or Windows
        //     desktop.
        Physical = 1,
        //
        // Summary:
        //     The device is virtual, such as the iOS simulators, Android emulators or Windows
        //     emulators.
        Virtual = 2
    }

    public class WelcomeDeviceMessage
    {
        public string DeviceIdentifier { get; set; }
        public string DeviceName { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string OSVersion { get; set; }
        public string Platform { get; set; }
        public string Idiom { get; set; }
        public DeviceType DeviceType { get; set; }
    }
}
