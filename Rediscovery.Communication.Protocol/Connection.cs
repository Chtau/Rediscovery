using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public class Connection
    {
        public string IP { get; set; }
        public int Port { get; set; }
    }

    public enum ConnectionState
    {
        Unkown,
        Active,
        Error
    }
}
