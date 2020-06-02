using CommunicationFeatureProvider.ProtoServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public static class CommunicationFeatureProviderExtensions
    {
        public static IApplicationBuilder UseFeatureProvider(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<FeatureExchangeService>();
            });
            return app;
        }

        public static IServiceCollection AddFeatureProvider<TActiveDeviceService, TFeatureEntityService>(this IServiceCollection services)
            where TActiveDeviceService : class, IActiveDeviceService
            where TFeatureEntityService : class, IFeatureEntityService
        {
            services.AddGrpc();
            services.AddSingleton<IActiveDeviceService, TActiveDeviceService>();
            services.AddSingleton<IFeatureEntityService, TFeatureEntityService>();
            services.AddSingleton<IFeatureResponseService, FeatureResponseService>();
            return services;
        }

        public static IServiceCollection AddFeatureProvider(this IServiceCollection services)
        {
            services.AddGrpc();
            return services;
        }
    }
}
