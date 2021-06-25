using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Handshake
{
    internal class AcknowledgeResult
    {
        public enum State
        {
            None,
            Ok,
            Timeout,
            Denied,
            Running
        }

        public State ResponseState { get; private set; } = State.None;
        public DateTime Start { get; private set; } = DateTime.UtcNow;
        public DateTime? End { get; private set; }
        public string RemoteDeviceIdentifer { get; }
        public string HandshakePassword { get; }
        public HandshakeState Response { get; private set; }

        public AcknowledgeResult(string remoteDeviceIdentifer, string password)
        {
            RemoteDeviceIdentifer = remoteDeviceIdentifer.ExactLength(16);
            HandshakePassword = password;
        }

        public void StartRequest(State state = State.Running)
        {
            Start = DateTime.UtcNow;
            ResponseState = state;
        }

        public void ResponseReceived(State state, HandshakeState response)
        {
            End = DateTime.UtcNow;
            ResponseState = state;
            Response = response;
        }
    }
}
