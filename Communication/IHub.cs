using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationConsumer
{
    public interface IHub
    {
        event EventHandler<List<SharedCoreModels.DeviceInfo>> ActiveDeviceInfoReceived;
        event EventHandler<List<SharedCoreModels.DeviceInfo>> DeviceInfoReceived;
        event EventHandler<List<SharedCoreModels.DeviceFeature>> ServiceFeatureReceived;
        event EventHandler<SharedCoreModels.LoggerEntryModel> LogEntryReceived;

        void Authenticate(string applicationKey, Models.ConnectionConfiguration configuration, Action<Models.ConnectionConfiguration, bool> callback);
        void Connect(string applicationKey, Models.ConnectionConfiguration configuration);
        bool RequestAllData();
    }
}
