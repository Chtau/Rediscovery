using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Logger
{
    public class RemoteLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
        public Features.RemoteResources.IRemoteResourcesLiveLogger RemoteResourcesLiveLogger { get; set; }
    }
}
