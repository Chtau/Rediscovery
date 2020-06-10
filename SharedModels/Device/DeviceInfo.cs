using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Device
{
    public class DeviceInfo : DeviceInfoMetadata
    {
        public Guid Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public bool AllowAccess { get; set; }
        public DateTime? RequestTime { get; set; }
    }
}
