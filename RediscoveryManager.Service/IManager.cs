using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RediscoveryManager.Service
{
    public interface IManager
    {
        Models.ManagerConnectionState ManagerConnectionState { get; }
        Models.CurrentConnection CurrentConnection { get; }
        SharedBase.Connection.Manifest Manifest { get; }
        ObservableCollection<SharedBase.Device.DeviceInfo> ActiveDevices { get; set; }
        ObservableCollection<SharedBase.Device.DeviceInfo> PendingDevices { get; set; }
        ObservableCollection<SharedBase.Device.DeviceInfo> Devices { get; set; }
        ObservableCollection<SharedBase.Device.FeatureDefinitionExtended> Features { get; set; }
        event EventHandler<SharedBase.Connection.Enums.ConnectionState> AfterConnecting;
        event EventHandler DeviceCollectionChanged;
        event EventHandler FeaturesCollectionChanged;
        event EventHandler<Guid> PendingDeviceResolved;
        event EventHandler ManifestChanged;
        bool TryConnect(string ip, int port, string deviceIdentifier);
        void Disconnect();
        void TryResolvePendingDevice(Guid deviceId, bool resolve);
        void TryDeleteDevice(Guid deviceId);
    }
}
