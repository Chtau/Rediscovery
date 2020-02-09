using DesktopFeatureConsole;
using DesktopFeatureMediaPlayer;
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
            OnDiscovery();
            Console.ReadLine();
        }

        private static void OnDiscovery()
        {
            var Client = new UdpClient();
            var RequestData = Encoding.ASCII.GetBytes("SomeRequestData");
            var ServerEp = new IPEndPoint(IPAddress.Any, 0);

            Client.EnableBroadcast = true;
            Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

            var ServerResponseData = Client.Receive(ref ServerEp);
            var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
            System.Diagnostics.Debug.Print("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString() + Environment.NewLine);

            Client.Close();
        }

        static void ConsoleFeature()
        {
            SharedCoreModels.DeviceFeature.IDeviceFeatureImplementation feature = new DeviceFeatureConsole();
            feature.SendData += (object sender, DeviceFeatureData e) =>
            {
                Console.WriteLine(e.Data);
            };
            feature.Init();
            feature.ReceiveData(new DeviceFeatureData { Data = "echo Hallo" });
            feature.ReceiveData(new DeviceFeatureData { Data = "echo World" });
        }

        static void MediaPlayerFeature()
        {
            foreach (var item in DeviceFeatureMediaPlayer.GetProfiles())
            {
                SharedCoreModels.DeviceFeature.IDeviceFeatureImplementation feature = new DeviceFeatureMediaPlayer(item);
                feature.SendData += (object sender, DeviceFeatureData e) =>
                {
                    Console.WriteLine(e.Data);
                };
                feature.Init();
                //System.Threading.Thread.Sleep(60000);
                feature.ReceiveData(new DeviceFeatureData { Data = new SharedCoreModels.FeatureModels.MediaPlayer.ClientCommandSendModel(new Guid("4D7A3004-F4F7-4B43-8DF1-2B9CA73F8991"), SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration.CommandTypes.VolumneUp) });
                //feature.ReceiveData(new DeviceFeatureData { Data = "echo World" });
            }
        }
    }
}
