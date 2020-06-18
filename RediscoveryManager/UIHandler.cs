using RediscoveryManager.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class UIHandler
    {
        private ConnectToServiceHandler connectToService = new ConnectToServiceHandler();
        private readonly IManager _manager;

        public UIHandler(IManager manager)
        {
            _manager = manager;
        }

        public void Start(string[] args)
        {
            TryParseConnectionArguments(args);
            SharedUI.DisplayDefaultTitle();
            string lastInput = null;
            do
            {

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
                connectToService.Handle(args);
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
    }
}
