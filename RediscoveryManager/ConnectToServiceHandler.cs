using CommunicationAuthenticationConsumer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class ConnectToServiceHandler
    {
        public string IP { get; private set; } = "";
        public int Port { get; private set; } = 0;
        public int PortSSL { get; private set; } = 0;
        public string DeviceIdentifier { get; private set; } = "";
        public string Token { get; private set; } = null;
        public string Pem { get; private set; } = null;
        public SharedBase.Connection.Enums.AllowConnect CanConnect { get; set; } = SharedBase.Connection.Enums.AllowConnect.None;
        public SharedBase.Connection.Enums.ConnectionState ConnectionState { get; set; } = SharedBase.Connection.Enums.ConnectionState.None;

        IAuthenticationConsumerService consumerService = new AuthenticationConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
        IGreetingConsumerService hand = new GreetingConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);

        public ConnectToServiceHandler()
        {
            consumerService.ReceivedManifestReply += (obj, args) =>
            {
                //Console.WriteLine("[ReceivedManifestReply] Client:" + args.ClientName);
            };
            consumerService.ReceivedWelcomeReply += (obj, args) =>
            {
                ConnectionState = args.State;
                if (ConnectionState == SharedBase.Connection.Enums.ConnectionState.OK)
                {
                    Token = args.Token;
                    //consumerService.RequestManifest(args.Token);
                }
                DisplayTitle();
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
                        IP = result;
                    }
                } else if (Commands.MatchInput(lastInput, Commands.SetPort))
                {
                    lastInput = null;
                    var result = SetProperty("Port");
                    if (!Commands.MatchInput(result, Commands.Abort))
                    {
                        if (int.TryParse(result, out int p))
                            Port = p;
                    }
                } else if (Commands.MatchInput(lastInput, Commands.SetDeviceIdentifier))
                {
                    lastInput = null;
                    var result = SetProperty("DeviceIdentifier");
                    if (!Commands.MatchInput(result, Commands.Abort))
                    {
                        DeviceIdentifier = result;
                    }
                } else if (Commands.MatchInput(lastInput, Commands.Connect))
                {
                    lastInput = null;
                    Connect();
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
                Value = IP
            });
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Port: ",
                Value = Port > 0 ? Port.ToString() : ""
            });
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Device Identifier: ",
                Value = DeviceIdentifier
            });
            Console.WriteLine();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Prefix = "Can connect:",
                Value = $"{CanConnect}",
                Color = CanConnect == SharedBase.Connection.Enums.AllowConnect.OK ? ConsoleColor.Green : ConsoleColor.White
            }, new ConsoleExtensions.WriteParams
            {
                Prefix = " Connected:",
                Value = $"{ConnectionState}",
                Color = ConnectionState == SharedBase.Connection.Enums.ConnectionState.OK ? ConsoleColor.Green : ConsoleColor.White
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

        private void Connect()
        {
            CanConnect = SharedBase.Connection.Enums.AllowConnect.None;
            ConnectionState = SharedBase.Connection.Enums.ConnectionState.None;
            Pem = null;
            Token = null;
            PortSSL = 0;

            var result = hand.GreetHost(IP, Port, new SharedBase.Connection.GreetingDeviceMessage
            {
                DeviceIdentifier = DeviceIdentifier,
                DeviceName = "",
                DeviceType = "",
                Idiom = "",
                Manufacturer = "",
                Model = "",
                OSVersion = "",
                Platform = ""
            });
            CanConnect = result.CanConnect;
            if (CanConnect == SharedBase.Connection.Enums.AllowConnect.OK)
            {
                Pem = result.PEM;
                PortSSL = result.SSLPort;
                consumerService.Connect(IP, PortSSL, Pem);
                consumerService.SendWelcome(new SharedBase.Connection.WelcomeDeviceMessage
                {
                    DeviceIdentifier = DeviceIdentifier,
                });
            }
            else
            {
                
            }
        }
    }
}
