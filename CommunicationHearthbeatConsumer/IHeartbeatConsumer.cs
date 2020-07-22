using CommunicationBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommunicationHeartbeatConsumer
{
    public interface IHeartbeatConsumer
    {
        int PingResponseWaitingMilliseconds { get; set; }
        event EventHandler<RoundTripResult> ReceivedBeatRoundtrip;
        bool Connect(ConsumerConnectionConfiguration connectionConfiguration);
        bool Disconnect();
        void StartBeat(string identifier, string token, CancellationTokenSource cts = null);
    }
}
