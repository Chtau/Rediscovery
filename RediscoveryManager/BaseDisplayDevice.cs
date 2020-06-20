using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public abstract class BaseDisplayDevice : BaseDisplay
    {


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
