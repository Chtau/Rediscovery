using RediscoveryManager.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class PendingDevicesHandler : BaseDisplay
    {
        private const string DisplayName = "pendingdevices";
        private readonly IManager _manager;
        private int currentNavigationIndex = 0;

        public PendingDevicesHandler(IManager manager)
        {
            _manager = manager;
            _manager.DeviceCollectionChanged += (obj, args) =>
            {
                if (string.Equals(SharedUI.CurrentDisplay, DisplayName))
                {
                    WaitForWriting();
                    if (currentNavigationIndex > _manager.PendingDevices.Count)
                        currentNavigationIndex = 0;
                    DisplayTitle();
                }
            };
        }

        public override void Handle(string[] args)
        {
            SharedUI.CurrentDisplay = DisplayName;
            DisplayTitle();
            string lastInput = null;
            do
            {
                if (_manager.PendingDevices?.Count > 0 && Commands.MatchInput(lastInput, Commands.Previous))
                {
                    lastInput = null;
                    if (currentNavigationIndex == 0)
                        currentNavigationIndex = _manager.PendingDevices.Count - 1;
                    else
                        currentNavigationIndex--;
                }
                else if (_manager.PendingDevices?.Count > 0 && Commands.MatchInput(lastInput, Commands.Next))
                {
                    lastInput = null;
                    currentNavigationIndex++;
                    if (currentNavigationIndex >= _manager.PendingDevices.Count)
                        currentNavigationIndex = 0;
                }
                else if (_manager.PendingDevices?.Count > 0 && Commands.MatchInput(lastInput, Commands.Accept))
                {
                    lastInput = null;
                    var item = _manager.PendingDevices[currentNavigationIndex];
                    _manager.TryResolvePendingDevice(item.Id, true);
                }
                else if (_manager.PendingDevices?.Count > 0 && Commands.MatchInput(lastInput, Commands.Deny))
                {
                    lastInput = null;
                    var item = _manager.PendingDevices[currentNavigationIndex];
                    _manager.TryResolvePendingDevice(item.Id, false);
                }
                else
                {
                    lastInput = Console.ReadKey().KeyChar.ToString();
                }
            } while (ResetOrBack(lastInput));
        }

        internal override void DisplayTitle()
        {
            isWriting = true;
            Console.Clear();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Pending Devices: ",
                Value = currentNavigationIndex.ToString() + " / " + _manager.PendingDevices?.Count.ToString()
            });
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{Commands.Previous.PutifyStringArray()} = Previous device");
            Console.WriteLine($"{Commands.Next.PutifyStringArray()} = Next device");
            Console.WriteLine($"{Commands.Accept.PutifyStringArray()} = Accept access request");
            Console.WriteLine($"{Commands.Deny.PutifyStringArray()} = Deny access request");
            Console.WriteLine();
            Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            Console.WriteLine();

            if (_manager.PendingDevices?.Count > 0)
            {
                var item = _manager.PendingDevices[currentNavigationIndex];
                Console.WriteLine();
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "Identifier: ",
                    Value = item.Identifier
                });
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "Name: ",
                    Value = item.Name
                });
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "Model: ",
                    Value = item.Model
                });
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "OS: ",
                    Value = item.OSVersion
                });
                ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                {
                    Color = ConsoleColor.White,
                    Prefix = "OS: ",
                    Value = $"{item.RequestTime}"
                });
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.Write("Command:");
            isWriting = false;
        }
    }
}
