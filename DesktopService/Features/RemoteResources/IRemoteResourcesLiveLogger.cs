using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.RemoteResources
{
    public interface IRemoteResourcesLiveLogger
    {
        [Obsolete("Use [CommunicationResourceProvider.IRemoteResourcesSenderService.SendLoggerEntry] instead")]
        void Log(LoggerEntryModel liveLoggerModel);
    }
}
