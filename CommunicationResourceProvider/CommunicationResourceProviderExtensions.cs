using CommunicationResourceProvider.ProtoServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public static class CommunicationResourceProviderExtensions
    {
        public static IApplicationBuilder UseResourceProvider(this IApplicationBuilder app)
        {
            var resourceRepository = app.ApplicationServices.GetRequiredService<IResourcesRepository>();
            var resourceManager = app.ApplicationServices.GetRequiredService<IResourceManager>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ResourceExchangeService>();
            });
            return app;
        }

        public static IServiceCollection AddResourceProvider<TResourcesRepository, TResourceManager>(this IServiceCollection services)
            where TResourcesRepository : class, IResourcesRepository
            where TResourceManager : class, IResourceManager
        {
            services.AddSingleton<IResourcesRepository, TResourcesRepository>();
            services.AddSingleton<IResourceManager, TResourceManager>();
            services.AddGrpc();
            return services;
        }
    }
}
