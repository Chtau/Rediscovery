using Rediscovery.Shared.Base.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Provider.Resource
{
    public interface IResourcesRepository
    {
        List<Rediscovery.Shared.Base.Device.FeatureDefinitionExtended> GetResourceDeviceFeature();
        List<Rediscovery.Shared.Base.Device.DeviceInfo> GetResourceDeviceInfo();
        List<string> GetResourceActiveDeviceIds();
        bool DeleteDeviceInfo(Guid id);
        Rediscovery.Shared.Base.Device.DeviceInfo UpdateDeviceInfo(Rediscovery.Shared.Base.Device.DeviceInfo deviceInfo);
        List<Rediscovery.Shared.Base.Device.DeviceInfo> GetResourcePendingAuthenticationDevices();
        bool ResolvePendingAuthenticationDevices(Guid deviceId, bool accept);
        List<FeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId);
        FeatureSetting GetResourceDeviceFeatureSettings(Guid featureId);
        List<Rediscovery.Shared.Base.Statistics.HeartbeatStatisticItem> GetHeartbeatStatistic();
        event EventHandler HeartbeatStatisticsChanged;
        List<Rediscovery.Shared.Base.Logging.LoggerEntry> GetLoggerEntires();
        event EventHandler LoggerEntiresChanged;
        event EventHandler HeartbeatActiveIDsChanged;
    }
}
