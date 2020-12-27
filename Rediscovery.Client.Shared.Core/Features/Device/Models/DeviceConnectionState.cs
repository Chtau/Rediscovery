using Rediscovery.Communication.Base;
using Rediscovery.Shared.Base.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Device.Models
{
    public class DeviceConnectionState
    {
        public enum StateChange
        {
            None,
            Probe,
            ProbeReply,
            GreetHost,
            GreetHostReply,
            Connect,
            ConnectReply,
            Welcome,
            WelcomeReply,
            ManifestReceived
        }

        public enum StateConnectReply
        {
            None,
            Ok,
            Failed
        }

        public StateChange Change { get; set; }
        public ConnectionConfiguration Configuration { get; set; }
        public StateConnectReply CurrentStateConnectReply { get; set; }
        public Enums.AllowConnect Allowed { get; set; }
        public Shared.Base.Connection.Enums.ConnectionState CurrentState { get; set; }
        public string Token { get; set; }
        public Manifest DeviceManifest { get; set; }
}
}
