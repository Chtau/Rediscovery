using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    [Obsolete("Use Pipeline & Communication instead")]
    internal interface IListener
    {
        void Initialize(Models.BaseConfiguration configuration);
        void StateCompleteListener(Action<StateComplete> callback);
        void Start();
        void Stop();
    }
}
