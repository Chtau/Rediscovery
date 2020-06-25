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
        private readonly ActiveDevicesHandler _activeDevicesHandler;
        private readonly ManifestHandler _manifestHandler;
        private readonly FeatureHandler _featureHandler;
        private readonly IManager _manager;

        public UIHandler(IManager manager)
        {
            SharedUI.CurrentDisplay = DisplayName;
            _manager = manager;
            _connectToService = new ConnectToServiceHandler(_manager);
            _pendingDevicesHandler = new PendingDevicesHandler(_manager);
            _allDevicesHandler = new AllDevicesHandler(_manager);
            _activeDevicesHandler = new ActiveDevicesHandler(_manager);
            _manifestHandler = new ManifestHandler(_manager);
            _featureHandler = new FeatureHandler(_manager);
            _manager.AfterConnecting += (obj, args) =>
            {
                if (string.Equals(SharedUI.CurrentDisplay, DisplayName))
                {
                    WaitForWriting();
                    DisplayDefaultTitle();
                }
            };
        }

        public void Start(SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration connectionConfiguration)
        {
            if (!string.IsNullOrWhiteSpace(connectionConfiguration.IP) || connectionConfiguration.Port > 0 || !string.IsNullOrWhiteSpace(connectionConfiguration.DeviceIdentifier))
            {
                _manager.SetConnectionValues(connectionConfiguration.IP, connectionConfiguration.Port, connectionConfiguration.DeviceIdentifier);
                if (connectionConfiguration.AutoConnect)
                    _manager.TryConnect();
            }
            string lastInput = null;
            do
            {
                SharedUI.CurrentDisplay = DisplayName;
                DisplayDefaultTitle();
                lastInput = Console.ReadLine();
                SwitchMenu(lastInput, connectionConfiguration);
            } while (SharedUI.ResetOrExit(lastInput));
        }

        private void SwitchMenu(string input, SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration connectionConfiguration)
        {
            if (Commands.MatchInput(input, Commands.Help))
            {

            } else if (Commands.MatchInput(input, Commands.Connect))
            {
                _connectToService.Handle();
            } else if (Commands.MatchInput(input, Commands.PendingDevices))
            {
                _pendingDevicesHandler.Handle();
            }
            else if (Commands.MatchInput(input, Commands.AllDevices))
            {
                _allDevicesHandler.Handle();
            }
            else if (Commands.MatchInput(input, Commands.ActiveDevices))
            {
                _activeDevicesHandler.Handle();
            }
            else if (Commands.MatchInput(input, Commands.Manifest))
            {
                _manifestHandler.Handle();
            }
            else if (Commands.MatchInput(input, Commands.Features))
            {
                _featureHandler.Handle();
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
            Console.WriteLine($"{Commands.ActiveDevices.PutifyStringArray()} = View active Devices");
            Console.WriteLine($"{Commands.PendingDevices.PutifyStringArray()} = Manage pending Device requests");
            Console.WriteLine($"{Commands.Manifest.PutifyStringArray()} = Service manifest");
            Console.WriteLine($"{Commands.Features.PutifyStringArray()} = Service features");
            Console.WriteLine($"{Commands.Exit.PutifyStringArray()} = Application exit");
            Console.WriteLine();
            Console.Write("Command:");
            isWriting = false;
        }
    }
}
