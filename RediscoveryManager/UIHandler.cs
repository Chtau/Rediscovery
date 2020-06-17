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
            //connectToService.TryParseArumgents(args);
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

        }
    }
}
