using CommunicationLoggerProvider;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.Logger
{
    public class RemoteLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
        public string LoggingModuleName { get; set; } = "Service";
        public Func<ILoggerHandler> GetLoggerHandlerInstance { get; set; }
    }
}
