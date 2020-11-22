using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Rediscovery.Client.App.Manager.Console
{
    public static class Arguments
    {
        public readonly static string[] SetIP = Commands.SetIP;
        public readonly static string[] SetPort = Commands.SetPort;
        public readonly static string[] SetDeviceIdentifier = Commands.SetDeviceIdentifier;
        public readonly static string[] Autoconnect = new string[] { "ac", "autoconnect" };

        public static string TryParseArgumentValue(string[] args, string[] keys)
        {
            foreach (var item in args)
            {
                if (item.Contains('='))
                {
                    var type = item.Split('=')[0].Replace("-", "");
                    var value = item.Split('=')[1];

                    if (Commands.MatchInput(type, keys))
                    {
                        return value?.Trim();
                    }
                }
            }
            return null;
        }

        public static bool TryParseArgumentMatch(string[] args, string[] keys)
        {
            foreach (var item in args)
            {
                if (Commands.MatchInput(item?.Replace("-", "")?.Trim(), keys))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
