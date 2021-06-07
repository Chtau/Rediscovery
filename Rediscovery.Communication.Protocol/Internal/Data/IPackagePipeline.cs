using Rediscovery.Communication.Protocol.Internal.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    internal interface IPackagePipeline
    {
        bool Outgoing<T>(T instance, DeviceGreetingReceived deviceGreeting);
        T Incoming<T>(byte[] raw);
        void SetIdentifier(string identifier);
    }
}
