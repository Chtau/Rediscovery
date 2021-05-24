using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class Device
    {
        public string Identifier { get; set; }
        public string FriendlyName { get; set; }
        public DeviceCommunicationSetting Data { get; set; }
        public DeviceCommunicationSetting LowData { get; set; }
    }
}
