using Rediscovery.Shared.Base.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Provider.Resource
{
    public interface IResourceManager
    {
        event EventHandler SendAllDevicesChanged;
        event EventHandler SendDevicesChanged;
        event EventHandler SendActiveDevicesChanged;
        event EventHandler SendPendingDevicesChanged;
        event EventHandler SendFeaturesChanged;
        event EventHandler SendHeartbeatChanged;
        event EventHandler SendLoggerEntriesChanged;
        void DeleteDevice(Guid deviceId);
        void UpdateDevice(Rediscovery.Shared.Base.Device.DeviceInfo deviceInfo);
        void ResolvePendingDevice(Guid deviceId, bool accept);
        void FeatureDetailProfileDelete(Guid featureId, string profileId);
        void FeatureDetailProfileSave(Guid featureId, FeatureProfil deviceFeatureProfil);
        void FeatureDetailSettingSave(Guid featureId, FeatureSetting deviceFeatureSetting);
    }
}
