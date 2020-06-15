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
        public readonly static string[] Back = new string[] { "b", "back" };
        public readonly static string[] Abort = new string[] { "a", "abort" };

        public readonly static string[] SetIP = new string[] { "ip" };
        public readonly static string[] SetPort = new string[] { "port", "p" };
        public readonly static string[] SetDeviceIdentifier = new string[] { "di", "deviceidentifier" };
    }
}
