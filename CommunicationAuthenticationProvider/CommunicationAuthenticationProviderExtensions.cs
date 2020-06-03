using CommunicationAuthenticationProvider.ProtoServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationProvider
{
    public static class CommunicationAuthenticationProviderExtensions
    {
        public static IApplicationBuilder UseAuthenticationProvider(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AuthenticationExchangeService>();
                endpoints.MapGrpcService<ManifestExchangeService>();
            });
            return app;
        }

        public static IServiceCollection AddAuthenticationProvider<TAuthenticationManager>(this IServiceCollection services)
            where TAuthenticationManager : class, IAuthenticationManager
        {
            services.AddGrpc();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<Services.IEventService, Services.EventService>();
            services.AddSingleton<IAuthenticationManager, TAuthenticationManager>();
            return services;
        }
    }
}
