using Rediscovery.Communication.Provider.Logger.ProtoService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Provider.Logger
{
    public static class CommunicationLoggerProviderExtensions
    {
        public static IApplicationBuilder UseLoggerProvider(this IApplicationBuilder app)
        {
            var directLogger = app.ApplicationServices.GetRequiredService<IDirectLogger>();
            var loggerHandler = app.ApplicationServices.GetRequiredService<ILoggerHandler>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<LoggerExchangeService>();
            });
            return app;
        }

        public static IServiceCollection AddLoggerProvider<TDirectLogger>(this IServiceCollection services)
            where TDirectLogger : class, IDirectLogger
        {
            services.AddSingleton<IDirectLogger, TDirectLogger>();
            services.AddSingleton<ILoggerHandler, LoggerHandler>();
            services.AddGrpc();
            return services;
        }
    }
}
