using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.Console
{
    public static class SharedUI
    {
        public static string CurrentDisplay = null;

        public static bool ResetOrExit(string input)
        {
            if (Commands.MatchInput(input, Commands.Exit))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static ConsoleColor AllowConnectToColor(Shared.Base.Connection.Enums.AllowConnect allowConnect)
        {
            switch (allowConnect)
            {
                case Shared.Base.Connection.Enums.AllowConnect.None:
                    return ConsoleColor.White;
                case Shared.Base.Connection.Enums.AllowConnect.OK:
                    return ConsoleColor.Green;
                case Shared.Base.Connection.Enums.AllowConnect.Error:
                    return ConsoleColor.Red;
                case Shared.Base.Connection.Enums.AllowConnect.Denied:
                    return ConsoleColor.Red;
                case Shared.Base.Connection.Enums.AllowConnect.UnkownDevice:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }

        public static ConsoleColor ConnectionStateToColor(Shared.Base.Connection.Enums.ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case Shared.Base.Connection.Enums.ConnectionState.None:
                    return ConsoleColor.White;
                case Shared.Base.Connection.Enums.ConnectionState.OK:
                    return ConsoleColor.Green;
                case Shared.Base.Connection.Enums.ConnectionState.Error:
                    return ConsoleColor.Red;
                case Shared.Base.Connection.Enums.ConnectionState.Warning:
                    return ConsoleColor.DarkYellow;
                case Shared.Base.Connection.Enums.ConnectionState.Offline:
                    return ConsoleColor.White;
                case Shared.Base.Connection.Enums.ConnectionState.Denied:
                    return ConsoleColor.Red;
                case Shared.Base.Connection.Enums.ConnectionState.WaitForApprovel:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
