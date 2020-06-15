using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public static class SharedUI
    {
        public static bool ResetOrExit(string input)
        {
            if (Commands.MatchInput(input, Commands.Exit))
            {
                return false;
            }
            else
            {
                DisplayDefaultTitle();
                return true;
            }
        }

        public static void DisplayDefaultTitle()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Rediscovery Manager");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine($"{Commands.Help.PutifyStringArray()} = shows help for the current context");
            Console.WriteLine($"{Commands.Connect.PutifyStringArray()} = Connect to Service");
            Console.WriteLine($"{Commands.Exit.PutifyStringArray()} = Application exit");
            Console.WriteLine();
            Console.WriteLine("Command:");
        }
    }
}
