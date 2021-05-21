using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    public interface IListener
    {
        void Initialize(Setting setting);
        void StateCompleteListener(Action<byte[]> callback);
        bool Start();
        bool Stop();
    }
}
