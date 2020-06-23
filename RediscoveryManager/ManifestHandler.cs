using RediscoveryManager.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
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

        public override void Handle(string[] args)
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
                    lastInput = Console.ReadLine();
                }
            } while (ResetOrBack(lastInput));
        }

        internal override void DisplayTitle()
        {
            isWriting = true;
            Console.Clear();
            Console.WriteLine("Service Manifest");
            Console.WriteLine();
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
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{Commands.Features.PutifyStringArray()} = View supported Features");
            Console.WriteLine();
            Console.WriteLine($"{Commands.Back.PutifyStringArray()} = Back to the main menu");
            Console.WriteLine();
            Console.Write("Command:");
            isWriting = false;
        }
    }
}
