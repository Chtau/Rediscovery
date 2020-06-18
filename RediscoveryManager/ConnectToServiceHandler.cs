using CommunicationAuthenticationConsumer;
using RediscoveryManager.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class ConnectToServiceHandler
    {
        private readonly IManager _manager;
        private bool isInternalConnectCall = false;

        public ConnectToServiceHandler(IManager manager)
        {
            _manager = manager;
            _manager.AfterConnecting += (obj, args) =>
            {
                if (isInternalConnectCall)
                {
                    DisplayTitle();
                }
            };
        }

        public void Handle(string[] args)
        {
            DisplayTitle();
            string lastInput = null;
            do
            {
                if (Commands.MatchInput(lastInput, Commands.SetIP))
                {
                    lastInput = null;
                    var result = SetProperty("IP Address");
                    if (!Commands.MatchInput(result, Commands.Abort))
                    {
                        _manager.CurrentConnection.IP = result;
                    }
                } else if (Commands.MatchInput(lastInput, Commands.SetPort))
                {
                    lastInput = null;
                    var result = SetProperty("Port");
                    if (!Commands.MatchInput(result, Commands.Abort))
                    {
                        if (int.TryParse(result, out int p))
                            _manager.CurrentConnection.Port = p;
                    }
                } else if (Commands.MatchInput(lastInput, Commands.SetDeviceIdentifier))
                {
                    lastInput = null;
                    var result = SetProperty("DeviceIdentifier");
                    if (!Commands.MatchInput(result, Commands.Abort))
                    {
                        _manager.CurrentConnection.DeviceIdentifier = result;
                    }
                } else if (Commands.MatchInput(lastInput, Commands.Connect))
                {
                    isInternalConnectCall = true;
                    lastInput = null;
                    _manager.TryConnect();
                } else
                {
                    lastInput = Console.ReadLine();
                }
            } while (ResetOrBack(lastInput));
        }

        private string SetProperty(string displayName)
        {
            Console.Clear();
            Console.WriteLine($"{Commands.Abort.PutifyStringArray()} = abort current action");
            Console.WriteLine();
            Console.WriteLine("Set property");
            Console.WriteLine();
            Console.Write($"{displayName}: ");
            return Console.ReadLine();
        }

        private bool ResetOrBack(string input)
        {
            if (Commands.MatchInput(input, Commands.Back))
            {
                return false;
            }
            else
            {
                DisplayTitle();
                return true;
            }
        }

        private void DisplayTitle()
        {
            Console.Clear();
            Console.WriteLine("Connect to Service");
            Console.WriteLine();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "IP: ",
                Value = _manager.CurrentConnection.IP
            });
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Port: ",
                Value = _manager.CurrentConnection.Port > 0 ? _manager.CurrentConnection.Port.ToString() : ""
            });
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Device Identifier: ",
                Value = _manager.CurrentConnection.DeviceIdentifier
            });
            Console.WriteLine();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Prefix = "Can connect:",
                Value = $"{_manager.ManagerConnectionState.CanConnect}",
                Color = AllowConnectToColor(_manager.ManagerConnectionState.CanConnect)
            }, new ConsoleExtensions.WriteParams
            {
                Prefix = " Connected:",
                Value = $"{_manager.ManagerConnectionState.ConnectionState}",
                Color = ConnectionStateToColor(_manager.ManagerConnectionState.ConnectionState)
            });
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{Commands.SetIP.PutifyStringArray()} = set the IP Address");
            Console.WriteLine($"{Commands.SetPort.PutifyStringArray()} = set the Port");
            Console.WriteLine($"{Commands.SetDeviceIdentifier.PutifyStringArray()} = set Device identifier");
            Console.WriteLine($"{Commands.Connect.PutifyStringArray()} = Establish a connection");
            Console.WriteLine();
            Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            Console.WriteLine();
            Console.Write("Command:");
        }

        private ConsoleColor AllowConnectToColor(SharedBase.Connection.Enums.AllowConnect allowConnect)
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

        private ConsoleColor ConnectionStateToColor(SharedBase.Connection.Enums.ConnectionState connectionState)
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
