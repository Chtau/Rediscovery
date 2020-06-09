using CommunicationResourceProvider;
using PluginFeature.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTestService.DummyImpl
{
    public class ResourceRepository : IResourcesRepository
    {
        private static List<DeviceInfo> deviceInfos = new List<DeviceInfo> {
                new DeviceInfo
                {
                    AllowAccess = true,
                    DeviceType = "A",
                    Id = Guid.NewGuid(),
                    Identifier = "Dev",
                    Idiom = "A",
                    Manufacturer = "A",
                    Model = "A",
                    Name = "Device",
                    OSVersion = "1",
                    Platform = "Vir"
                }
            };
        private static List<SharedBase.Device.FeatureDefinitionExtended> featureDefinitionExtendeds = new List<SharedBase.Device.FeatureDefinitionExtended>
            {
                new SharedBase.Device.FeatureDefinitionExtended
                {
                    Author = "A",
                    ControlIntegrationPoint = SharedBase.Device.IntegrationPoint.Mobile,
                    DisplayName = "A",
                    Documentation = null,
                    FeatureIntegrationPoint = SharedBase.Device.IntegrationPoint.Desktop,
                    HasProfiles = true,
                    HasSettings = true,
                    Id = Guid.NewGuid(),
                    MinimalControlIntegrationPoint = new SharedBase.Core.Version() {  Major = 0, Minor = 0, Patch = 0 },
                    MinimalFeatureIntegrationPoint = new SharedBase.Core.Version() {  Major = 0, Minor = 0, Patch = 0 },
                    Version = new SharedBase.Core.Version() {  Major = 0, Minor = 0, Patch = 0 },
                    PluginDirectory = null,
                    ProfileUIElementName = null,
                    ProfileUIReadonly = true,
                    SettingUIElementName = null,
                    SettingUIReadonly = true,
                    Website = null
                }
            };
        private static List<DeviceFeatureProfil> deviceFeatureProfils = new List<DeviceFeatureProfil>
            {
                new DeviceFeatureProfil
                {
                    DisplayName = "A",
                    FeatureId = Guid.NewGuid(),
                    Id = "0",
                    ProfileData = null
                }
            };
        private static DeviceFeatureSetting deviceFeatureSetting = new DeviceFeatureSetting
        {
            Data = null,
            FeatureId = Guid.NewGuid()
        };

        public bool DeleteDeviceInfo(Guid id)
        {
            return true;
        }

        public bool DeleteFeatureProfile(Guid featureId, string profileId)
        {
            return true;
        }

        public List<DeviceInfo> GetResourceActiveDeviceInfo()
        {
            return deviceInfos;
        }

        public List<SharedBase.Device.FeatureDefinitionExtended> GetResourceDeviceFeature()
        {
            return featureDefinitionExtendeds;
        }

        public List<DeviceFeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId)
        {
            return deviceFeatureProfils;
        }

        public byte[] GetResourceDeviceFeatureProfilesUI(Guid featureId)
        {
            return new byte[255];
        }

        public DeviceFeatureSetting GetResourceDeviceFeatureSettings(Guid featureId)
        {
            return deviceFeatureSetting;
        }

        public byte[] GetResourceDeviceFeatureSettingsUI(Guid featureId)
        {
            return new byte[255];
        }

        public List<DeviceInfo> GetResourceDeviceInfo()
        {
            return deviceInfos;
        }

        public List<DeviceInfo> GetResourcePendingAuthenticationDevices()
        {
            return deviceInfos;
        }

        public bool ResolvePendingAuthenticationDevices(Guid deviceId, bool accept)
        {
            return true;
        }

        public bool SaveFeatureProfile(Guid featureId, DeviceFeatureProfil deviceFeatureProfil)
        {
            return true;
        }

        public bool SaveFeatureSettings(Guid featureId, DeviceFeatureSetting deviceFeatureSetting)
        {
            return true;
        }

        public DeviceInfo UpdateDeviceInfo(DeviceInfo deviceInfo)
        {
            return deviceInfos.First();
        }
    }
}
