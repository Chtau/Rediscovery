using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
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
                Console.Write(item.Prefix);
                Console.ForegroundColor = item.Color;
                Console.Write(item.Value);
                Console.ResetColor();
            }
            Console.Write(Environment.NewLine);
        }
    }
}
