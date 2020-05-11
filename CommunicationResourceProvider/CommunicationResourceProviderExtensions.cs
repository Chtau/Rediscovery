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
        public static IApplicationBuilder UseResourceProvider(this IApplicationBuilder app, string hubPath)
        {
            app.UseSignalR(x =>
            {
                x.MapHub<RemoteResourceHub>(hubPath);
            });
            return app;
        }

        public static IServiceCollection AddResourceProvider<TAuthenticateService, TResourcesRepository>(this IServiceCollection services)
            where TAuthenticateService : class, IAuthenticateService
            where TResourcesRepository : class, IResourcesRepository
        {
            services.AddSingleton<IAuthenticateService, TAuthenticateService>();
            services.AddSingleton<IResourcesRepository, TResourcesRepository>();
            services.AddSingleton<IRemoteResourcesSenderService, RemoteResourcesSenderService>();
            return services;
        }
    }
}
