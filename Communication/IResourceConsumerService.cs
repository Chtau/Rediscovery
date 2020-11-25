using Rediscovery.Communication.Base;
using Rediscovery.Shared.Base.Feature;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rediscovery.Communication.Consumer.Resource
{
    public interface IResourceConsumerService
    {
        event EventHandler<List<Shared.Base.Device.DeviceInfo>> ReceiveActiveDevices;
        event EventHandler<List<Shared.Base.Device.DeviceInfo>> ReceivePendingDevices;
        event EventHandler<List<Shared.Base.Device.DeviceInfo>> ReceiveDevices;
        event EventHandler<List<Shared.Base.Device.FeatureDefinitionExtended>> ReceiveFeatures;
        event EventHandler<Shared.Base.Device.DeviceInfo> ReceiveUpdateDevices;
        event EventHandler<(Guid deviceId, bool result)> ReceiveDeleteDevicesResult;
        event EventHandler<(Guid deviceId, bool accept)> ReceiveResolvePendingDevicesResult;
        event EventHandler<Models.FeatureDetail> ReceiveFeatureDetails;
        event EventHandler<List<Shared.Base.Statistics.HeartbeatStatisticItem>> ReceiveHeartbeatStatistic;
        event EventHandler<List<Shared.Base.Logging.LoggerEntry>> ReceiveLoggerEntires;

        bool Connect(ConsumerConnectionConfiguration connectionConfiguration);
        void ListenActiveDevices(string token, CancellationTokenSource cts = null);
        void ListenPendingDevices(string token, CancellationTokenSource cts = null);
        void ListenDevices(string token, CancellationTokenSource cts = null);
        void ListenFeatures(string token, CancellationTokenSource cts = null);
        void UpdateDevice(string token, Shared.Base.Device.DeviceInfo deviceInfo);
        void DeleteDevice(string token, Guid deviceId);
        void ResolvePendingDevice(string token, Guid deviceId, bool accept);
        void FeatureDetail(string token, FeatureSetting setting);
        void ListenHeartbeatStatistic(string token, CancellationTokenSource cts = null);
        void ListenLoggerEntires(string token, CancellationTokenSource cts = null);
    }
}
