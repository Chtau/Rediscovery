using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.RemoteResources
{
    public interface IRemoteResourcesLiveLogger
    {
        void Log(SharedBase.Logging.LoggerEntry liveLoggerModel);
    }
}
