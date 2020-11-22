using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.Console
{
    public class ManifestHandler : BaseDisplay
    {
        private const string DisplayName = "manifest";
        private readonly IManager _manager;

        public ManifestHandler(IManager manager)
        {
            _manager = manager;
            _manager.ManifestChanged += (obj, args) =>
            {
                WaitForWriting();
            };
        }

        public override void Handle()
        {
            SharedUI.CurrentDisplay = DisplayName;
            DisplayTitle();
            string lastInput = null;
            do
            {
                if (Commands.MatchInput(lastInput, Commands.Features))
                {
                    lastInput = null;
                }
                else
                {
                    lastInput = System.Console.ReadLine();
                }
            } while (ResetOrBack(lastInput));
        }

        internal override void DisplayTitle()
        {
            isWriting = true;
            System.Console.Clear();
            System.Console.WriteLine("Service Manifest");
            System.Console.WriteLine();
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Name: ",
                Value = _manager.Manifest?.ClientName
            });
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Version: ",
                Value = _manager.Manifest?.ClientVersion.ToString()
            });
            ConsoleExtensions.Write(new ConsoleExtensions.WriteParams
            {
                Color = ConsoleColor.Green,
                Prefix = "Minimum App Version: ",
                Value = _manager.Manifest?.AppMinimumVersion.ToString()
            });
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            System.Console.WriteLine();
            System.Console.Write("Command:");
            isWriting = false;
        }
    }
}
