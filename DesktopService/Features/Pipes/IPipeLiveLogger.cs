using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Pipes
{
    public interface IPipeLiveLogger
    {
        void Log(LoggerEntryModel liveLoggerModel);
    }
}
