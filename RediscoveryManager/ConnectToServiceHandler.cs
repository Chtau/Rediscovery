using CommunicationAuthenticationConsumer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class ConnectToServiceHandler
    {
        private bool isAutoConnectCall = false;

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
                if (!isAutoConnectCall)
                {
                    DisplayTitle();
                }
            };
        }

        public void TryParseArumgents(string[] args)
        {
            bool autoConnect = false;
            foreach (var item in args)
            {
                if (item.Contains('='))
                {
                    var type = item.Split('=')[0].Replace("-", "");
                    var value = item.Split('=')[1];

                    if (Commands.MatchInput(type, Commands.SetIP))
                    {
                        IP = value?.Trim();
                    }
                    else if (Commands.MatchInput(type, Commands.SetPort))
                    {
                        if (int.TryParse(value?.Trim(), out int p))
                            Port = p;
                    }
                    else if (Commands.MatchInput(type, Commands.SetDeviceIdentifier))
                    {
                        DeviceIdentifier = value?.Trim();
                    }
                } else
                {
                    if (string.Equals(item?.Replace("-", "")?.Trim(), "autoconnect"))
                    {
                        autoConnect = true;
                    }
                }
            }
            if (autoConnect)
            {
                if (!string.IsNullOrWhiteSpace(IP)
                    && Port > 0
                    && !string.IsNullOrWhiteSpace(DeviceIdentifier))
                {
                    Connect();
                    isAutoConnectCall = true;
                }
            }
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
                    isAutoConnectCall = false;
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
                Color = AllowConnectToColor(CanConnect)
            }, new ConsoleExtensions.WriteParams
            {
                Prefix = " Connected:",
                Value = $"{ConnectionState}",
                Color = ConnectionStateToColor(ConnectionState)
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
