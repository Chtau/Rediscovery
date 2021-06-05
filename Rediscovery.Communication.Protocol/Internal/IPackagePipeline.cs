using Rediscovery.Communication.Protocol.Internal.Listener;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal interface IPackagePipeline
    {
        event EventHandler<OutgoingPackageRawPart> SendNextRaw;
        bool Outgoing<T>(T instance, DeviceGreetingReceived deviceGreeting);
        T Incoming<T>(byte[] raw);
        void SetIdentifier(string identifier);
    }
}
