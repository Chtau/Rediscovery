using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationProvider
{
    public static class CommunicationAuthenticationProviderExtensions
    {
        public static IApplicationBuilder UseAuthenticationProvider(this IApplicationBuilder app, string hubPath)
        {
            app.UseSignalR(x =>
            {
                x.MapHub<AuthenticationHub>(hubPath);
            });
            return app;
        }

        public static IServiceCollection AddAuthenticationProvider<TAuthenticationManager>(this IServiceCollection services)
            where TAuthenticationManager : class, IAuthenticationManager
        {
            services.AddSingleton<IAuthenticationManager, TAuthenticationManager>();
            return services;
        }
    }
}
