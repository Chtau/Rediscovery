using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public interface IResourceManager
    {
        event EventHandler SendAllDevicesChanged;
        event EventHandler SendDevicesChanged;
        event EventHandler SendActiveDevicesChanged;
        event EventHandler SendPendingDevicesChanged;
        event EventHandler SendFeaturesChanged;
        void DeleteDevice(Guid deviceId);
        void UpdateDevice(SharedBase.Device.DeviceInfo deviceInfo);
        void ResolvePendingDevice(Guid deviceId, bool accept);
        void FeatureDetailProfileDelete(Guid featureId, string profileId);
        void FeatureDetailProfileSave(Guid featureId, FeatureProfil deviceFeatureProfil);
        void FeatureDetailSettingSave(Guid featureId, FeatureSetting deviceFeatureSetting);
        
    }
}
