using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public class Transfer<T>
    {
        public T Content { get; }
        public string DeviceIdentifier { get; }

        public Transfer(string deviceIdentifier, T content)
        {
            DeviceIdentifier = deviceIdentifier;
            Content = content;
        }
    }

    public enum TransportState
    {
        Unkown,
        Ok,
        Error,
        MissingPeer
    }
}
