using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.InternalLogger
{
    public class InternalLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
        public ConsoleColor Color { get; set; } = ConsoleColor.Yellow;
    }
}
