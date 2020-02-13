using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.PipeLogger
{
    public static class PipeLoggerExtensions
    {
        public static ILoggerFactory AddPipeLogger(this ILoggerFactory loggerFactory, PipeLoggerConfiguration config)
        {
            loggerFactory.AddProvider(new PipeLoggerProvider(config));
            return loggerFactory;
        }
        public static ILoggerFactory AddPipeLogger(this ILoggerFactory loggerFactory)
        {
            var config = new PipeLoggerConfiguration();
            return loggerFactory.AddPipeLogger(config);
        }
        public static ILoggerFactory AddPipeLogger(this ILoggerFactory loggerFactory, Action<PipeLoggerConfiguration> configure)
        {
            var config = new PipeLoggerConfiguration();
            configure(config);
            return loggerFactory.AddPipeLogger(config);
        }
    }
}
