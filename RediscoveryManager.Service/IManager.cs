using Rediscovery.Communication.Consumer.Heartbeat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Client.App.Manager
{
    public interface IManager
    {
        Models.ManagerConnectionState ManagerConnectionState { get; }
        Models.CurrentConnection CurrentConnection { get; set; }
        Shared.Base.Connection.Manifest Manifest { get; }
        ObservableCollection<Shared.Base.Device.DeviceInfo> ActiveDevices { get; set; }
        ObservableCollection<Shared.Base.Device.DeviceInfo> PendingDevices { get; set; }
        ObservableCollection<Shared.Base.Device.DeviceInfo> Devices { get; set; }
        ObservableCollection<Shared.Base.Device.FeatureDefinitionExtended> Features { get; set; }
        ObservableCollection<Shared.Base.Statistics.HeartbeatStatisticItem> HeartbeatStatistics { get; set; }
        ObservableCollection<Shared.Logging.Models.LoggerEntry> LoggerEntires { get; set; }
        event EventHandler<Shared.Base.Connection.Enums.ConnectionState> AfterConnecting;
        event EventHandler<Shared.Base.Connection.Enums.AllowConnect> GreetingsReply;
        event EventHandler<RoundTripResult> RoundTripReceived;
        event EventHandler DeviceCollectionChanged;
        event EventHandler FeaturesCollectionChanged;
        event EventHandler<Guid> PendingDeviceResolved;
        event EventHandler ManifestChanged;
        event EventHandler HeartbeatStatisticsChanged;
        event EventHandler LoggerEntiresChanged;
        event EventHandler<Shared.Logging.Commands.LogCommandConfigResult> LoggerCommandExecuted;
        void SetConnectionValues(string ip, int port, string deviceIdentifier);
        bool TryConnect();
        void Disconnect();
        void TryResolvePendingDevice(Guid deviceId, bool resolve);
        void TryDeleteDevice(Guid deviceId);
        void RemoteLogEntry(Shared.Logging.Models.LoggerEntry loggerEntry);
        bool RemoteLogExecuteCommand(Shared.Logging.Commands.LogCommandConfig logCommandConfig);
    }
}
