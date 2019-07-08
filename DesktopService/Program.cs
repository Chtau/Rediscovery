using System;

namespace DesktopService
{
    class Program
    {
        static void Main(string[] args)
        {
            var net = new NetworkDiscoveryService();

            Console.WriteLine("Hello World!");
            net.Server();
            Console.ReadKey();
        }
    }
}
