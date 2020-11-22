using CommunicationAuthenticationConsumer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Client.App.Manager.Console
{
    public class ConnectToServiceHandler : BaseDisplay
    {
        private const string DisplayName = "connect";
        private readonly IManager _manager;

        public ConnectToServiceHandler(IManager manager)
        {
            _manager = manager;
            _manager.AfterConnecting += (obj, args) =>
            {
                if (string.Equals(SharedUI.CurrentDisplay, DisplayName))
                {
                    WaitForWriting();
                    DisplayTitle();
                }
            };
        }

        public override void Handle()
        {
            SharedUI.CurrentDisplay = DisplayName;
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
                    lastInput = null;
                    _manager.TryConnect();
                } else
                {
                    lastInput = System.Console.ReadLine();
                }
            } while (ResetOrBack(lastInput));
        }

        private string SetProperty(string displayName)
        {
            System.Console.Clear();
            System.Console.WriteLine($"{Commands.Abort.PutifyStringArray()} = abort current action");
            System.Console.WriteLine();
            System.Console.WriteLine("Set property");
            System.Console.WriteLine();
            System.Console.Write($"{displayName}: ");
            return System.Console.ReadLine();
        }

        internal override void DisplayTitle()
        {
            isWriting = true;
            System.Console.Clear();
            System.Console.WriteLine("Connect to Service");
            System.Console.WriteLine();
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
            System.Console.WriteLine();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Prefix = "Can connect:",
                Value = $"{_manager.ManagerConnectionState.CanConnect}",
                Color = SharedUI.AllowConnectToColor(_manager.ManagerConnectionState.CanConnect)
            }, new ConsoleExtensions.WriteParams
            {
                Prefix = " Connected:",
                Value = $"{_manager.ManagerConnectionState.ConnectionState}",
                Color = SharedUI.ConnectionStateToColor(_manager.ManagerConnectionState.ConnectionState)
            });
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine($"{Commands.SetIP.PutifyStringArray()} = set the IP Address");
            System.Console.WriteLine($"{Commands.SetPort.PutifyStringArray()} = set the Port");
            System.Console.WriteLine($"{Commands.SetDeviceIdentifier.PutifyStringArray()} = set Device identifier");
            System.Console.WriteLine($"{Commands.Connect.PutifyStringArray()} = Establish a connection");
            System.Console.WriteLine();
            System.Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            System.Console.WriteLine();
            System.Console.Write("Command:");
            isWriting = false;
        }
    }
}
