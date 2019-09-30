using DesktopFeatureConsole;
using System;

namespace DesktopFeatureTestApp
{
    class ProgramConsole
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test");

            SharedCoreModels.DeviceFeature.IDeviceFeatureImplementation feature = new DeviceFeatureConsole();
            feature.SendData += (object sender, object e) =>
            {
                Console.WriteLine(e);
            };
            feature.Init();
            feature.ReceiveData("echo Hallo");
            feature.ReceiveData("echo World");

            Console.ReadLine();
        }
    }
}
