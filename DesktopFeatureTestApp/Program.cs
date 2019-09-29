using DesktopFeatureConsole;
using System;

namespace DesktopFeatureTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test");

            SharedCoreModels.DeviceFeature.IDeviceFeatureImplementation<string> feature = new DeviceFeatureConsole();
            feature.SendData += (object sender, string e) =>
            {
                Console.WriteLine(e);
                //Console.Write(e);
            };
            feature.Init();
            feature.ReceiveData("echo Hallo");
            feature.ReceiveData("echo World");

            Console.ReadLine();
            /*do
            {
                System.Threading.Thread.Sleep(500);
            } while (true);*/
        }
    }
}
