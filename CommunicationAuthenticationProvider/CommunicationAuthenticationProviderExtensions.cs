using Rediscovery.Communication.Authentication.Provider.ProtoServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Authentication.Provider
{
    public static class CommunicationAuthenticationProviderExtensions
    {
        internal static byte[] TokenSigningKey;

        public static IApplicationBuilder UseAuthenticationProvider(this IApplicationBuilder app)
        {
            var tokenService = app.ApplicationServices.GetRequiredService<Services.ITokenService>();
            var authenticationManager = app.ApplicationServices.GetRequiredService<IAuthenticationManager>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AuthenticationExchangeService>();
                endpoints.MapGrpcService<ManifestExchangeService>();
                endpoints.MapGrpcService<HandShakeExchangeService>();
            });

            return app;
        }

        public static IServiceCollection AddAuthenticationProvider<TAuthenticationManager>(this IServiceCollection services, string tokenSigningKeySecret, string deviceRole = "device", string resourceConsumerRole = "resourceconsumer")
            where TAuthenticationManager : class, IAuthenticationManager
        {
            TokenSigningKey = Encoding.ASCII.GetBytes(tokenSigningKeySecret);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeviceAndConsumer", policy =>
                {
                    if (!string.IsNullOrWhiteSpace(deviceRole) || !string.IsNullOrWhiteSpace(resourceConsumerRole))
                    {
                        policy.RequireRole(deviceRole, resourceConsumerRole);
                    }
                        
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy("Device", policy =>
                {
                    if (!string.IsNullOrWhiteSpace(deviceRole))
                        policy.RequireRole(deviceRole);
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy("ResourceConsumer", policy =>
                {
                    if (!string.IsNullOrWhiteSpace(resourceConsumerRole))
                        policy.RequireRole(resourceConsumerRole);
                    policy.RequireAuthenticatedUser();
                });
            });
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
            services.AddSingleton<Services.ITokenService, Services.TokenService>();
            services.AddSingleton<IAuthenticationManager, TAuthenticationManager>();
            return services;
        }
    }
}
