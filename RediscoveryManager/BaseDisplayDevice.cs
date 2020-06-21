using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RediscoveryManager
{
    public abstract class BaseDisplayDevice : BaseDisplay
    {
        internal int currentNavigationIndex = 0;

        internal virtual void WriteMenu()
        {
            
        }

        internal virtual string WriteTitle()
        {
            return "";
        }

        internal virtual IList<SharedBase.Device.DeviceInfo> DeviceCollection()
        {
            return new List<SharedBase.Device.DeviceInfo>();
        }

        internal override void DisplayTitle()
        {
            isWriting = true;
            Console.Clear();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = WriteTitle(),
                Value = (currentNavigationIndex + 1).ToString() + " / " + DeviceCollection()?.Count.ToString()
            });
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{Commands.Previous.PutifyStringArray()} = Previous device");
            Console.WriteLine($"{Commands.Next.PutifyStringArray()} = Next device");
            WriteMenu();
            Console.WriteLine();
            Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            Console.WriteLine();

            if (DeviceCollection()?.Count > 0)
            {
                var item = DeviceCollection()[currentNavigationIndex];
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
