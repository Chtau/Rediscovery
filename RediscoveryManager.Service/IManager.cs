using CommunicationHeartbeatConsumer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RediscoveryManager.Service
{
    public interface IManager
    {
        Models.ManagerConnectionState ManagerConnectionState { get; }
        Models.CurrentConnection CurrentConnection { get; set; }
        SharedBase.Connection.Manifest Manifest { get; }
        ObservableCollection<SharedBase.Device.DeviceInfo> ActiveDevices { get; set; }
        ObservableCollection<SharedBase.Device.DeviceInfo> PendingDevices { get; set; }
        ObservableCollection<SharedBase.Device.DeviceInfo> Devices { get; set; }
        ObservableCollection<SharedBase.Device.FeatureDefinitionExtended> Features { get; set; }
        ObservableCollection<SharedBase.Statistics.HeartbeatStatisticItem> HeartbeatStatistics { get; set; }
        ObservableCollection<SharedBase.Logging.LoggerEntry> LoggerEntires { get; set; }
        event EventHandler<SharedBase.Connection.Enums.ConnectionState> AfterConnecting;
        event EventHandler<SharedBase.Connection.Enums.AllowConnect> GreetingsReply;
        event EventHandler<RoundTripResult> RoundTripReceived;
        event EventHandler DeviceCollectionChanged;
        event EventHandler FeaturesCollectionChanged;
        event EventHandler<Guid> PendingDeviceResolved;
        event EventHandler ManifestChanged;
        event EventHandler HeartbeatStatisticsChanged;
        event EventHandler LoggerEntiresChanged;
        event EventHandler<SharedBase.Logging.LogCommandConfigResult> LoggerCommandExecuted;
        void SetConnectionValues(string ip, int port, string deviceIdentifier);
        bool TryConnect();
        void Disconnect();
        void TryResolvePendingDevice(Guid deviceId, bool resolve);
        void TryDeleteDevice(Guid deviceId);
        void RemoteLogEntry(SharedBase.Logging.LoggerEntry loggerEntry);
        bool RemoteLogExecuteCommand(SharedBase.Logging.LogCommandConfig logCommandConfig);
    }
}
