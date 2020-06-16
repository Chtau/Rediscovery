using CommunicationAuthenticationConsumer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class ConnectToServiceHandler
    {
        private string ip = "";
        private int port = 0;
        private int portSSL = 0;
        private string deviceIdentifier = "";
        string token = null;
        string pem = null;
        SharedBase.Connection.Enums.AllowConnect canConnect = SharedBase.Connection.Enums.AllowConnect.None;
        SharedBase.Connection.Enums.ConnectionState connectionState = SharedBase.Connection.Enums.ConnectionState.None;

        IAuthenticationConsumerService consumerService = new AuthenticationConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);


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
                Value = ip
            });
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Port: ",
                Value = port > 0 ? port.ToString() : ""
            });
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Device Identifier: ",
                Value = deviceIdentifier
            });
            Console.WriteLine();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Prefix = "Can connect:",
                Value = $"{canConnect}",
                Color = canConnect == SharedBase.Connection.Enums.AllowConnect.OK ? ConsoleColor.Green : ConsoleColor.White
            }, new ConsoleExtensions.WriteParams
            {
                Prefix = " Connected:",
                Value = $"{connectionState}",
                Color = connectionState == SharedBase.Connection.Enums.ConnectionState.OK ? ConsoleColor.Green : ConsoleColor.White
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
            consumerService.ReceivedManifestReply += (obj, args) =>
            {
                //Console.WriteLine("[ReceivedManifestReply] Client:" + args.ClientName);
            };
            consumerService.ReceivedWelcomeReply += (obj, args) =>
            {
                connectionState = args.State;
                //Console.WriteLine($"[ReceivedWelcomeReply] Token:{args.Token} State:{args.State}");
                if (connectionState == SharedBase.Connection.Enums.ConnectionState.OK)
                {
                    token = args.Token;
                    //consumerService.RequestManifest(args.Token);
                }
                else
                {
                    /*Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ReceivedWelcomeReply] No authorization! State:{args.State}");
                    Console.ResetColor();*/
                }
                DisplayTitle();
            };
            var hand = new GreetingConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            var result = hand.GreetHost(ip, port, new SharedBase.Connection.GreetingDeviceMessage
            {
                DeviceIdentifier = deviceIdentifier,
                DeviceName = "",
                DeviceType = "",
                Idiom = "",
                Manufacturer = "",
                Model = "",
                OSVersion = "",
                Platform = ""
            });
            canConnect = result.CanConnect;
            if (canConnect == SharedBase.Connection.Enums.AllowConnect.OK)
            {
                pem = result.PEM;
                portSSL = result.SSLPort;
                consumerService.Connect(ip, portSSL, pem);
                consumerService.SendWelcome(new SharedBase.Connection.WelcomeDeviceMessage
                {
                    DeviceIdentifier = deviceIdentifier,
                });
            }
            else
            {
                
            }
        }
    }
}
