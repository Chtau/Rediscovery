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
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGrpcService<AuthenticationExchangeService>();
            });
            return app;
        }

        public static IServiceCollection AddResourceProvider<TResourcesRepository>(this IServiceCollection services)
            where TResourcesRepository : class, IResourcesRepository
        {
            services.AddSingleton<IResourcesRepository, TResourcesRepository>();
            services.AddGrpc();
            return services;
        }
    }
}
