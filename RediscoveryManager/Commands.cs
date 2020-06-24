using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public static class Commands
    {
        public static bool MatchInput(string input, params string[] commands)
        {
            foreach (var item in commands)
            {
                if (string.Equals(input, item))
                    return true;
            }
            return false;
        }

        public readonly static string[] Exit = new string[] { "exit", "quit", "q" };
        public readonly static string[] Help = new string[] { "h", "help", "?" };
        public readonly static string[] Connect = new string[] { "c", "connect" };
        public readonly static string[] PendingDevices = new string[] { "pd", "pendingdevices" };
        public readonly static string[] AllDevices = new string[] { "d", "devices" };
        public readonly static string[] ActiveDevices = new string[] { "ad", "activedevices" };
        public readonly static string[] Manifest = new string[] { "m", "manifest" };
        public readonly static string[] Features = new string[] { "f", "features" };
        public readonly static string[] Back = new string[] { "b", "back" };
        public readonly static string[] Abort = new string[] { "a", "abort" };

        public readonly static string[] SetIP = new string[] { "ip" };
        public readonly static string[] SetPort = new string[] { "p", "port" };
        public readonly static string[] SetDeviceIdentifier = new string[] { "di", "deviceidentifier" };

        public readonly static string[] Previous = new string[] { "p", "-" };
        public readonly static string[] Next = new string[] { "n", "+" };
        public readonly static string[] Accept = new string[] { "a" };
        public readonly static string[] Deny = new string[] { "d" };
    }
}
