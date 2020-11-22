using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Connection
{
    public static class Enums
    {
        public enum ConnectionState
        {
            None = 0,
            OK = 1,
            Error = 2,
            Warning = 3,
            Offline = 4,
            Denied = 5,
            WaitForApprovel = 6
        }

        public enum AllowConnect
        {
            None = 0,
            OK = 1,
            Error = 2,
            Denied = 3,
            UnkownDevice = 4,
        }
    }
}
