using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public static class CommunicationFeatureProviderExtensions
    {
        public static IApplicationBuilder UseFeatureProvider(this IApplicationBuilder app, string hubPath)
        {
            app.UseSignalR(x =>
            {
                x.MapHub<FeatureHub>(hubPath);
            });
            return app;
        }

        public static IServiceCollection AddFeatureProvider<TActiveDeviceService, TFeatureEntityService>(this IServiceCollection services)
            where TActiveDeviceService : class, IActiveDeviceService
            where TFeatureEntityService : class, IFeatureEntityService
        {
            services.AddSingleton<IActiveDeviceService, TActiveDeviceService>();
            services.AddSingleton<IFeatureEntityService, TFeatureEntityService>();
            services.AddSingleton<IFeatureResponseService, FeatureResponseService>();
            return services;
        }
    }
}
