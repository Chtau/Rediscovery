using RediscoveryManager.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RediscoveryManager
{
    public class UIHandler : BaseDisplay
    {
        private const string DisplayName = "main";
        private readonly ConnectToServiceHandler _connectToService;
        private readonly PendingDevicesHandler _pendingDevicesHandler;
        private readonly AllDevicesHandler _allDevicesHandler;
        private readonly IManager _manager;

        public UIHandler(IManager manager)
        {
            SharedUI.CurrentDisplay = DisplayName;
            _manager = manager;
            _connectToService = new ConnectToServiceHandler(_manager);
            _pendingDevicesHandler = new PendingDevicesHandler(_manager);
            _allDevicesHandler = new AllDevicesHandler(_manager);
            _manager.AfterConnecting += (obj, args) =>
            {
                if (string.Equals(SharedUI.CurrentDisplay, DisplayName))
                {
                    WaitForWriting();
                    DisplayDefaultTitle();
                }
            };
        }

        public void Start(string[] args)
        {
            TryParseConnectionArguments(args);
            string lastInput = null;
            do
            {
                SharedUI.CurrentDisplay = DisplayName;
                DisplayDefaultTitle();
                lastInput = Console.ReadLine();
                SwitchMenu(lastInput, args);
            } while (SharedUI.ResetOrExit(lastInput));
        }

        private void SwitchMenu(string input, string[] args)
        {
            if (Commands.MatchInput(input, Commands.Help))
            {

            } else if (Commands.MatchInput(input, Commands.Connect))
            {
                _connectToService.Handle(args);
            } else if (Commands.MatchInput(input, Commands.PendingDevices))
            {
                _pendingDevicesHandler.Handle(args);
            }
            else if (Commands.MatchInput(input, Commands.AllDevices))
            {
                _allDevicesHandler.Handle(args);
            }
        }

        private void TryParseConnectionArguments(string[] args)
        {
            int port = 0;
            var deviceIdentifier = Arguments.TryParseArgumentValue(args, Arguments.SetDeviceIdentifier);
            var ip = Arguments.TryParseArgumentValue(args, Arguments.SetIP);
            var portString = Arguments.TryParseArgumentValue(args, Arguments.SetPort);
            int.TryParse(portString, out port);
            var autoConnect = Arguments.TryParseArgumentMatch(args, Arguments.Autoconnect);
            if (!string.IsNullOrWhiteSpace(ip) || port > 0 || !string.IsNullOrWhiteSpace(deviceIdentifier))
            {
                _manager.SetConnectionValues(ip, port, deviceIdentifier);
                if (autoConnect)
                    _manager.TryConnect();
            }
        }

        private void DisplayDefaultTitle()
        {
            isWriting = true;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Rediscovery Manager");
            Console.ResetColor();
            Console.WriteLine();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Prefix = "Connected:",
                Value = $"{_manager.ManagerConnectionState.ConnectionState}",
                Color = SharedUI.ConnectionStateToColor(_manager.ManagerConnectionState.ConnectionState)
            });
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{Commands.Help.PutifyStringArray()} = shows help for the current context");
            Console.WriteLine($"{Commands.Connect.PutifyStringArray()} = Connect to Service");
            Console.WriteLine($"{Commands.AllDevices.PutifyStringArray()} = View all Devices");
            Console.WriteLine($"{Commands.PendingDevices.PutifyStringArray()} = Manage pending Device requests");
            Console.WriteLine($"{Commands.Exit.PutifyStringArray()} = Application exit");
            Console.WriteLine();
            Console.Write("Command:");
            isWriting = false;
        }
    }
}
