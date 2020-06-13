using CommunicationResourceProvider;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTestService.DummyImpl
{
    public class ResourceManager : IResourceManager
    {
        public event EventHandler SendAllDevicesChanged;
        public event EventHandler SendDevicesChanged;
        public event EventHandler SendActiveDevicesChanged;
        public event EventHandler SendPendingDevicesChanged;
        public event EventHandler SendFeaturesChanged;

        public ResourceManager()
        {
            Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(5000);
                    SendAllDevicesChanged?.Invoke(this, EventArgs.Empty);
                } while (true);
            });
        }

        public void DeleteDevice(Guid deviceId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{nameof(DeleteDevice)}]");
            Console.ResetColor();
        }

        public void FeatureDetailProfileDelete(Guid featureId, string profileId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{nameof(FeatureDetailProfileDelete)}]");
            Console.ResetColor();
        }

        public void FeatureDetailProfileSave(Guid featureId, FeatureProfil deviceFeatureProfil)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{nameof(FeatureDetailProfileSave)}]");
            Console.ResetColor();
        }

        public void FeatureDetailSettingSave(Guid featureId, FeatureSetting deviceFeatureSetting)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{nameof(FeatureDetailSettingSave)}]");
            Console.ResetColor();
        }

        public void ResolvePendingDevice(Guid deviceId, bool accept)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{nameof(ResolvePendingDevice)}]");
            Console.ResetColor();
        }

        public void UpdateDevice(SharedBase.Device.DeviceInfo deviceInfo)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{nameof(UpdateDevice)}]");
            Console.ResetColor();
        }
    }
}
