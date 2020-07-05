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
        bool Connect(string ipAddress, int port, string certificatePEM);
        bool Disconnect();
        void StartBeat(string token, CancellationTokenSource cts = null);
    }
}
