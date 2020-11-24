using Rediscovery.Communication.Heartbeat.Provider.ProtoServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Heartbeat.Provider
{
    public static class CommunicationHeartbeatProviderExtensions
    {
        public static IApplicationBuilder UseHeartbeatProvider(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var heartbeatActive = app.ApplicationServices.GetRequiredService<IHeartbeatActive>();
            var heartbeatStatistic = app.ApplicationServices.GetRequiredService<IHeartbeatStatistic>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<HeartbeatExchangeService>();
            });
            return app;
        }

        public static IServiceCollection AddHeartbeatProvider<TConfiguration>(this IServiceCollection services)
            where TConfiguration : class, IConfiguration
        {
            services.AddGrpc();
            services.AddSingleton<IConfiguration, TConfiguration>();
            services.AddSingleton<IHeartbeatActive, HeartbeatActive>();
            services.AddSingleton<IHeartbeatStatistic, HeartbeatStatistic>();
            return services;
        }
    }
}
