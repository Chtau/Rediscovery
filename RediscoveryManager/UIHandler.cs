using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rediscovery.Client.App.Manager.Console
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
                lastInput = System.Console.ReadLine();
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
            System.Console.Clear();
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Rediscovery Manager");
            System.Console.ResetColor();
            System.Console.WriteLine();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Prefix = "Connected:",
                Value = $"{_manager.ManagerConnectionState.ConnectionState}",
                Color = SharedUI.ConnectionStateToColor(_manager.ManagerConnectionState.ConnectionState)
            });
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine($"{Commands.Help.PutifyStringArray()} = shows help for the current context");
            System.Console.WriteLine($"{Commands.Connect.PutifyStringArray()} = Connect to Service");
            System.Console.WriteLine($"{Commands.AllDevices.PutifyStringArray()} = View all Devices");
            System.Console.WriteLine($"{Commands.ActiveDevices.PutifyStringArray()} = View active Devices");
            System.Console.WriteLine($"{Commands.PendingDevices.PutifyStringArray()} = Manage pending Device requests");
            System.Console.WriteLine($"{Commands.Manifest.PutifyStringArray()} = Service manifest");
            System.Console.WriteLine($"{Commands.Features.PutifyStringArray()} = Service features");
            System.Console.WriteLine($"{Commands.Exit.PutifyStringArray()} = Application exit");
            System.Console.WriteLine();
            System.Console.Write("Command:");
            isWriting = false;
        }
    }
}
