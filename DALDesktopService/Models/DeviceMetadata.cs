using System;
using System.Collections.Generic;
using System.Text;

namespace DALDesktopService.Models
{
    public class DeviceMetadata
    {
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string OSVersion { get; set; }
        public string Platform { get; set; }
        public string Idiom { get; set; }
        public string DeviceType { get; set; }
    }
}
