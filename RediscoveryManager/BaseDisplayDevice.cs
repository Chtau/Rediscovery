using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Rediscovery.Client.App.Manager.Console
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

        internal virtual string DisplayIdentifierName()
        {
            return "";
        }

        internal virtual IList<SharedBase.Device.DeviceInfo> DeviceCollection()
        {
            return new List<SharedBase.Device.DeviceInfo>();
        }

        internal virtual bool HandleSubMenu(string lastInput)
        {
            return false;
        }

        internal override void DisplayTitle()
        {
            isWriting = true;
            System.Console.Clear();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = WriteTitle(),
                Value = OnGetTitlePageIndex()
            });
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine($"{Commands.Previous.PutifyStringArray()} = Previous device");
            System.Console.WriteLine($"{Commands.Next.PutifyStringArray()} = Next device");
            WriteMenu();
            System.Console.WriteLine();
            System.Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            System.Console.WriteLine();

            if (DeviceCollection()?.Count > 0)
            {
                var item = DeviceCollection()[currentNavigationIndex];
                System.Console.WriteLine();
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
                if (item.RequestTime.HasValue)
                {
                    ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
                    {
                        Color = ConsoleColor.White,
                        Prefix = "Requested at: ",
                        Value = $"{item.RequestTime}"
                    });
                }

                System.Console.WriteLine();
                System.Console.WriteLine();
            }

            System.Console.Write("Command:");
            isWriting = false;
        }

        private string OnGetTitlePageIndex()
        {
            string retVal = "";
            if (DeviceCollection() == null || DeviceCollection().Count == 0)
            {
                retVal = " no devices";
            } else
            {
                retVal = (currentNavigationIndex + 1).ToString() + " / " + DeviceCollection()?.Count.ToString();
            }
            
            return retVal;
        }

        public override void Handle()
        {
            SharedUI.CurrentDisplay = DisplayIdentifierName();
            DisplayTitle();
            string lastInput = null;
            do
            {
                if (DeviceCollection()?.Count > 0 && Commands.MatchInput(lastInput, Commands.Previous))
                {
                    lastInput = null;
                    if (currentNavigationIndex == 0)
                        currentNavigationIndex = DeviceCollection().Count - 1;
                    else
                        currentNavigationIndex--;
                }
                else if (DeviceCollection()?.Count > 0 && Commands.MatchInput(lastInput, Commands.Next))
                {
                    lastInput = null;
                    currentNavigationIndex++;
                    if (currentNavigationIndex >= DeviceCollection().Count)
                        currentNavigationIndex = 0;
                }
                else
                {
                    if (HandleSubMenu(lastInput))
                        lastInput = null;
                    else
                        lastInput = System.Console.ReadKey().KeyChar.ToString();
                }
            } while (ResetOrBack(lastInput));
        }
    }
}
