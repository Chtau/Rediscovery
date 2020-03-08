using DesktopFeatureConsole;
using DesktopFeatureMediaPlayer;
using PluginFeature.Interfaces;
using PluginFeature.Models;
using SharedCoreModels.DeviceFeature;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DesktopFeatureTestApp
{
    class ProgramConsole
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test");

            //ConsoleFeature();
            //MediaPlayerFeature();
            Console.ReadLine();
        }

        static void ConsoleFeature()
        {
            IDeviceFeatureImplementation feature = new DeviceFeatureConsole();
            feature.SendData += (object sender, DeviceFeatureData e) =>
            {
                Console.WriteLine(e.Data);
            };
            feature.Init(null, null);
            feature.ReceiveData(new DeviceFeatureData { Data = "echo Hallo" });
            feature.ReceiveData(new DeviceFeatureData { Data = "echo World" });
        }

        static void MediaPlayerFeature()
        {
            foreach (var item in DeviceFeatureMediaPlayer.GetProfiles())
            {
                IDeviceFeatureImplementation feature = new DeviceFeatureMediaPlayer();
                feature.SendData += (object sender, DeviceFeatureData e) =>
                {
                    Console.WriteLine(e.Data);
                };
                feature.Init(null, null);
                //System.Threading.Thread.Sleep(60000);
                feature.ReceiveData(new DeviceFeatureData { Data = new SharedCoreModels.FeatureModels.MediaPlayer.ClientCommandSendModel(new Guid("4D7A3004-F4F7-4B43-8DF1-2B9CA73F8991"), null, SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration.CommandTypes.VolumneUp) });
                //feature.ReceiveData(new DeviceFeatureData { Data = "echo World" });
            }
        }
    }
}
