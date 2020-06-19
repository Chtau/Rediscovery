using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
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

        public static ConsoleColor AllowConnectToColor(SharedBase.Connection.Enums.AllowConnect allowConnect)
        {
            switch (allowConnect)
            {
                case SharedBase.Connection.Enums.AllowConnect.None:
                    return ConsoleColor.White;
                case SharedBase.Connection.Enums.AllowConnect.OK:
                    return ConsoleColor.Green;
                case SharedBase.Connection.Enums.AllowConnect.Error:
                    return ConsoleColor.Red;
                case SharedBase.Connection.Enums.AllowConnect.Denied:
                    return ConsoleColor.Red;
                case SharedBase.Connection.Enums.AllowConnect.UnkownDevice:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }

        public static ConsoleColor ConnectionStateToColor(SharedBase.Connection.Enums.ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case SharedBase.Connection.Enums.ConnectionState.None:
                    return ConsoleColor.White;
                case SharedBase.Connection.Enums.ConnectionState.OK:
                    return ConsoleColor.Green;
                case SharedBase.Connection.Enums.ConnectionState.Error:
                    return ConsoleColor.Red;
                case SharedBase.Connection.Enums.ConnectionState.Warning:
                    return ConsoleColor.DarkYellow;
                case SharedBase.Connection.Enums.ConnectionState.Offline:
                    return ConsoleColor.White;
                case SharedBase.Connection.Enums.ConnectionState.Denied:
                    return ConsoleColor.Red;
                case SharedBase.Connection.Enums.ConnectionState.WaitForApprovel:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
