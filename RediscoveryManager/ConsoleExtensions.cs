using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.Console
{
    public static class ConsoleExtensions
    {
        public class WriteParams
        {
            public string Prefix { get; set; }
            public string Value { get; set; }
            public ConsoleColor Color { get; set; }
        }

        public static void Write(params WriteParams[] writeParams)
        {
            foreach (var item in writeParams)
            {
                System.Console.Write(item.Prefix);
                System.Console.ForegroundColor = item.Color;
                System.Console.Write(item.Value);
                System.Console.ResetColor();
            }
            System.Console.Write(Environment.NewLine);
        }
    }
}
