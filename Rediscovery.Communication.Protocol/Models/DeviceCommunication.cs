using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DeviceCommunication
    {
        public DeviceCommunicationSetting Data { get; set; }
        public DeviceCommunicationSetting Large { get; set; }
        public DeviceCommunicationSetting Handshake { get; set; }
    }
}
