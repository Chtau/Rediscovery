using DesktopFeatureConsole;
using DesktopFeatureVLC;
using SharedCoreModels.DeviceFeature;
using System;

namespace DesktopFeatureTestApp
{
    class ProgramConsole
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test");

            /*SharedCoreModels.DeviceFeature.IDeviceFeatureImplementation feature = new DeviceFeatureConsole();
            feature.SendData += (object sender, DeviceFeatureData e) =>
            {
                Console.WriteLine(e.Data);
            };
            feature.Init();
            feature.ReceiveData(new DeviceFeatureData { Data = "echo Hallo" });
            feature.ReceiveData(new DeviceFeatureData { Data = "echo World" });*/

            SharedCoreModels.DeviceFeature.IDeviceFeatureImplementation feature = new DeviceFeatureVLC();
            feature.Init();
            Console.ReadLine();
        }
    }
}
