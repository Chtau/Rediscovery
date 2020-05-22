using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public interface IRemoteResourcesSenderService
    {
        void AddActiveDevice(string userId);
        void RemoveActiveDevice(string userId);
        void SendActiveDeviceInfo();
        void SendDeviceInfo();
        void SendServiceFeature();
        void SendPendingAuthenticationDevices();
        void SendLoggerEntry(SharedCoreModels.LoggerEntryModel liveLoggerModel);
        void SendFeatureDetails(Guid featureId);
        void SendFeatureDetailsUI(Guid featureId);
    }
}
