using CommunicationHearthbeatProvider.ProtoServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationHeartbeatProvider
{
    public static class CommunicationHeartbeatProviderExtensions
    {
        public static IApplicationBuilder UseHeartbeatProvider(this IApplicationBuilder app)
        {
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
            services.AddSingleton<IHeartbeatStatistic, HeartbeatStatistic>();
            return services;
        }
    }
}
