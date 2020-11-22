using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.Logger
{
    public static class RemoteLoggerExtensions
    {
        public static ILoggerFactory AddRemoteLogger(this ILoggerFactory loggerFactory, RemoteLoggerConfiguration config)
        {
            loggerFactory.AddProvider(new RemoteLoggerProvider(config));
            return loggerFactory;
        }
        public static ILoggerFactory AddRemoteLogger(this ILoggerFactory loggerFactory)
        {
            var config = new RemoteLoggerConfiguration();
            return loggerFactory.AddRemoteLogger(config);
        }
        public static ILoggerFactory AddRemoteLogger(this ILoggerFactory loggerFactory, Action<RemoteLoggerConfiguration> configure)
        {
            var config = new RemoteLoggerConfiguration();
            configure(config);
            return loggerFactory.AddRemoteLogger(config);
        }
    }
}
