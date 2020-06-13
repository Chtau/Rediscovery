using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommunicationResourceConsumer
{
    public interface IResourceConsumerService
    {
        event EventHandler<List<SharedBase.Device.DeviceInfo>> ReceiveActiveDevices;
        event EventHandler<List<SharedBase.Device.DeviceInfo>> ReceivePendingDevices;
        event EventHandler<List<SharedBase.Device.DeviceInfo>> ReceiveDevices;
        event EventHandler<List<SharedBase.Device.FeatureDefinitionExtended>> ReceiveFeatures;
        event EventHandler<SharedBase.Device.DeviceInfo> ReceiveUpdateDevices;
        event EventHandler<(Guid deviceId, bool result)> ReceiveDeleteDevicesResult;
        event EventHandler<(Guid deviceId, bool accept)> ReceiveResolvePendingDevicesResult;
        event EventHandler<(Guid featureId, string profileId, bool result)> ReceiveFeatureDetailProfileDeleteResult;
        event EventHandler<(FeatureProfil profile, bool result)> ReceiveFeatureDetailProfileSave;
        event EventHandler<(FeatureSetting setting, bool result)> ReceiveFeatureDetailSettingSave;
        event EventHandler<Models.FeatureDetail> ReceiveFeatureDetails;

        bool Connect(string ipAddress, int port, string certificatePEM);
        void ListenActiveDevices(string token, CancellationTokenSource cts = null);
        void ListenPendingDevices(string token, CancellationTokenSource cts = null);
        void ListenDevices(string token, CancellationTokenSource cts = null);
        void ListenFeatures(string token, CancellationTokenSource cts = null);
        void UpdateDevice(string token, SharedBase.Device.DeviceInfo deviceInfo);
        void DeleteDevice(string token, Guid deviceId);
        void ResolvePendingDevice(string token, Guid deviceId, bool accept);
        void FeatureDetailProfileDelete(string token, Guid featureId, string profileId);
        void FeatureDetailProfileSave(string token, FeatureProfil profil);
        void FeatureDetailSettingSave(string token, FeatureSetting setting);
        void FeatureDetail(string token, FeatureSetting setting);
    }
}
