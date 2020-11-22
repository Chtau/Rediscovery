using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.InternalLogger
{
    public static class InternalLoggerExtensions
    {
        public static ILoggerFactory AddInternalLogger(this ILoggerFactory loggerFactory, InternalLoggerConfiguration config)
        {
            loggerFactory.AddProvider(new InternalLoggerProvider(config));
            return loggerFactory;
        }
        public static ILoggerFactory AddInternalLogger(this ILoggerFactory loggerFactory)
        {
            var config = new InternalLoggerConfiguration();
            return loggerFactory.AddInternalLogger(config);
        }
        public static ILoggerFactory AddInternalLogger(this ILoggerFactory loggerFactory, Action<InternalLoggerConfiguration> configure)
        {
            var config = new InternalLoggerConfiguration();
            configure(config);
            return loggerFactory.AddInternalLogger(config);
        }
    }
}
