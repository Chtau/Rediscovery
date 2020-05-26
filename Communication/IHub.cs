using CommunicationBase;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationResourceConsumer
{
    public interface IHub
    {
        event EventHandler<List<SharedCoreModels.DeviceInfo>> ActiveDeviceInfoReceived;
        event EventHandler<List<SharedCoreModels.DeviceInfo>> DeviceInfoReceived;
        event EventHandler<List<SharedCoreModels.DeviceInfo>> PendingAuthenticationDeviceReceived;
        event EventHandler<List<SharedCoreModels.DeviceFeature>> ServiceFeatureReceived;
        event EventHandler<SharedCoreModels.LoggerEntryModel> LogEntryReceived;
        event EventHandler<bool> ConnectionStateChanged;
        event EventHandler<SharedCoreModels.EntityContent<Guid, byte[]>> FeatureProfileUIReceived;
        event EventHandler<SharedCoreModels.EntityContent<Guid, byte[]>> FeatureSettingUIReceived;
        event EventHandler<SharedCoreModels.EntityContent<Guid, List<DeviceFeatureProfil>>> FeatureProfilesReceived;
        event EventHandler<SharedCoreModels.EntityContent<Guid, DeviceFeatureSetting>> FeatureSettingsReceived;

        void Init(ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP);
        void Authenticate(string applicationKey, ConnectionConfiguration configuration, Action<ConnectionConfiguration, bool> callback);
        void Connect(string applicationKey, ConnectionConfiguration configuration, Action<bool> listenerCallback);
        bool RequestAllData();
        Task<bool> Disconnect();
        void RequestResolvePendingAuthenticationDevice(Guid deviceId, bool accept);
        void RequestDeleteDevice(SharedCoreModels.DeviceInfo deviceInfo);
        void RequestUpdateDevice(SharedCoreModels.DeviceInfo deviceInfo);
        void RequestFeatureDetailsUI(Guid featureId);
        void RequestFeatureSaveProfile(SharedCoreModels.EntityContent<Guid, DeviceFeatureProfil> profileEntity);
        void RequestFeatureDeleteProfile(SharedCoreModels.EntityContent<Guid, DeviceFeatureProfil> profileEntity);
        void RequestFeatureSaveSetting(SharedCoreModels.EntityContent<Guid, DeviceFeatureSetting> settingEntity);
    }
}
