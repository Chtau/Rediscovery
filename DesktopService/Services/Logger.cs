using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Services
{
    public class Logger : ILogger
    {
        public void Diagnostic(string msg, string module = null)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(module + " : " + msg);
            Console.ResetColor();
        }

        public void Exception(Exception ex, string module = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(module + " : " + ex.ToString());
            Console.ResetColor();
        }

        public void Info(string msg, string module = null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(module + " : " + msg);
            Console.ResetColor();
        }

        public void Warning(string msg, string module = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(module + " : " + msg);
            Console.ResetColor();
        }
    }
}
