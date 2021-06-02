using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal interface IListener
    {
        void Initialize(Models.BaseConfiguration configuration);
        void StateCompleteListener(Action<StateComplete> callback);
        void Start();
        void Stop();
    }
}
