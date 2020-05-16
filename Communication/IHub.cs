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

        void Init(ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP);
        void Authenticate(string applicationKey, Models.ConnectionConfiguration configuration, Action<Models.ConnectionConfiguration, bool> callback);
        void Connect(string applicationKey, Models.ConnectionConfiguration configuration, Action<bool> listenerCallback);
        bool RequestAllData();
        void Disconnect();
        void RequestResolvePendingAuthenticationDevice(Guid deviceId, bool accept);
        void RequestDeleteDevice(SharedCoreModels.DeviceInfo deviceInfo);
        void RequestUpdateDevice(SharedCoreModels.DeviceInfo deviceInfo);
    }
}
