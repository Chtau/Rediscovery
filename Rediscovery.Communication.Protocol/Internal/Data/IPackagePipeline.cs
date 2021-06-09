using Rediscovery.Communication.Protocol.Internal.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    internal interface IPackagePipeline
    {
        bool Outgoing<T>(T instance, DeviceGreetingReceived deviceGreeting);
        void Incoming<T>(Action<T, string> instanceCallback);
        void SetIdentifier(string identifier);
    }
}
