using CommunicationResourceProvider;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTestService.DummyImpl
{
    public class ResourceRepository : IResourcesRepository
    {
        private static List<SharedBase.Device.DeviceInfo> deviceInfos = new List<SharedBase.Device.DeviceInfo> {
                new SharedBase.Device.DeviceInfo
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
                    Id = Guid.NewGuid(),
                    MinimalControlIntegrationPoint = new SharedBase.Core.Version() {  Major = 0, Minor = 0, Patch = 0 },
                    MinimalFeatureIntegrationPoint = new SharedBase.Core.Version() {  Major = 0, Minor = 0, Patch = 0 },
                    Version = new SharedBase.Core.Version() {  Major = 0, Minor = 0, Patch = 0 },
                    PluginDirectory = null,
                    HasProfilConfiguration = false,
                    HasSettingConfiguration = false,
                    Website = null
                }
            };
        private static List<FeatureProfil> deviceFeatureProfils = new List<FeatureProfil>
            {
                new FeatureProfil
                {
                    DisplayName = "A",
                    FeatureId = Guid.NewGuid(),
                    Id = "0",
                    ProfileData = null
                }
            };
        private static FeatureSetting deviceFeatureSetting = new FeatureSetting
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

        public List<SharedBase.Device.DeviceInfo> GetResourceActiveDeviceInfo()
        {
            return deviceInfos;
        }

        public List<SharedBase.Device.FeatureDefinitionExtended> GetResourceDeviceFeature()
        {
            return featureDefinitionExtendeds;
        }

        public List<FeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId)
        {
            return deviceFeatureProfils;
        }

        public byte[] GetResourceDeviceFeatureProfilesUI(Guid featureId)
        {
            return new byte[255];
        }

        public FeatureSetting GetResourceDeviceFeatureSettings(Guid featureId)
        {
            return deviceFeatureSetting;
        }

        public byte[] GetResourceDeviceFeatureSettingsUI(Guid featureId)
        {
            return new byte[255];
        }

        public List<SharedBase.Device.DeviceInfo> GetResourceDeviceInfo()
        {
            return deviceInfos;
        }

        public List<SharedBase.Device.DeviceInfo> GetResourcePendingAuthenticationDevices()
        {
            return deviceInfos;
        }

        public bool ResolvePendingAuthenticationDevices(Guid deviceId, bool accept)
        {
            return true;
        }

        public bool SaveFeatureProfile(Guid featureId, FeatureProfil deviceFeatureProfil)
        {
            return true;
        }

        public bool SaveFeatureSettings(Guid featureId, FeatureSetting deviceFeatureSetting)
        {
            return true;
        }

        public SharedBase.Device.DeviceInfo UpdateDeviceInfo(SharedBase.Device.DeviceInfo deviceInfo)
        {
            return deviceInfos.First();
        }
    }
}
