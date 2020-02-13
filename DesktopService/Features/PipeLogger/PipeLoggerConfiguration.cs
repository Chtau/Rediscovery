using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.PipeLogger
{
    public class PipeLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
        public Features.Pipes.IPipeLiveLogger PipeLiveLogger { get; set; }
    }
}
