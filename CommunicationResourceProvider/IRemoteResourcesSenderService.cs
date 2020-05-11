using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public interface IRemoteResourcesSenderService
    {
        void SendActiveDeviceInfo();
        void SendDeviceInfo();
        void SendServiceFeature();
        void SendLoggerEntry(SharedCoreModels.LoggerEntryModel liveLoggerModel);
    }
}
