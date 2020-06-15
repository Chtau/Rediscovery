using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class ConnectToServiceHandler
    {
        private string ip = "";
        private int port = 0;
        private string deviceIdentifier = "";

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
                        ip = result;
                    }
                } else if (Commands.MatchInput(lastInput, Commands.SetPort))
                {
                    lastInput = null;
                    var result = SetProperty("Port");
                    if (!Commands.MatchInput(result, Commands.Abort))
                    {
                        if (int.TryParse(result, out int p))
                            port = p;
                    }
                } else if (Commands.MatchInput(lastInput, Commands.SetDeviceIdentifier))
                {
                    lastInput = null;
                    var result = SetProperty("DeviceIdentifier");
                    if (!Commands.MatchInput(result, Commands.Abort))
                    {
                        deviceIdentifier = result;
                    }
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connect to Service");
            Console.ResetColor();
            Console.Write("IP:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(ip);
            Console.ResetColor();
            Console.Write(Environment.NewLine);
            Console.Write("Port:");
            Console.ForegroundColor = ConsoleColor.Green;
            if (port > 0)
                Console.Write(port.ToString());
            Console.ResetColor();
            Console.Write(Environment.NewLine);
            Console.Write("Device Identifier:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(deviceIdentifier);
            Console.ResetColor();
            Console.Write(Environment.NewLine);
            Console.WriteLine();
            Console.WriteLine($"{Commands.SetIP.PutifyStringArray()} = set the IP Address");
            Console.WriteLine($"{Commands.SetPort.PutifyStringArray()} = set the Port");
            Console.WriteLine($"{Commands.SetDeviceIdentifier.PutifyStringArray()} = set Device identifier");
            Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            Console.WriteLine();
            Console.WriteLine("Command:");
        }
    }
}
