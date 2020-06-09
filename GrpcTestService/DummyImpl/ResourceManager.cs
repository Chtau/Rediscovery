using CommunicationResourceProvider;
using PluginFeature.Models;
using SharedCoreModels;
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

        public void FeatureDetailProfileSave(Guid featureId, DeviceFeatureProfil deviceFeatureProfil)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{nameof(FeatureDetailProfileSave)}]");
            Console.ResetColor();
        }

        public void FeatureDetailSettingSave(Guid featureId, DeviceFeatureSetting deviceFeatureSetting)
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

        public void UpdateDevice(DeviceInfo deviceInfo)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{nameof(UpdateDevice)}]");
            Console.ResetColor();
        }
    }
}
