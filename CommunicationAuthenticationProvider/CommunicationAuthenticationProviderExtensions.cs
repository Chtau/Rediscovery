using CommunicationAuthenticationProvider.ProtoServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationAuthenticationProvider
{
    public static class CommunicationAuthenticationProviderExtensions
    {
        internal static byte[] TokenSigningKey;

        public static IApplicationBuilder UseAuthenticationProvider(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AuthenticationExchangeService>();
                endpoints.MapGrpcService<ManifestExchangeService>();
            });
            return app;
        }

        public static IServiceCollection AddAuthenticationProvider<TAuthenticationManager>(this IServiceCollection services, string tokenSigningKeySecret)
            where TAuthenticationManager : class, IAuthenticationManager
        {
            TokenSigningKey = Encoding.ASCII.GetBytes(tokenSigningKeySecret);
            services.AddAuthorization();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(TokenSigningKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddGrpc();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<Services.IEventService, Services.EventService>();
            services.AddSingleton<Services.ITokenService, Services.TokenService>();
            services.AddSingleton<IAuthenticationManager, TAuthenticationManager>();
            return services;
        }
    }
}
