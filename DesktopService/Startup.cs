using DesktopService.Features.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService
{
    public class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            /*var host = new HostBuilder();
            host.RunConsoleAsync().GetAwaiter().GetResult();*/
        }

        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var identitySettingsSection = Configuration.GetSection("IdentitySettings");
            services.Configure<Features.Identity.Models.IdentitySettings>(identitySettingsSection);

            var appSettings = identitySettingsSection.Get<Features.Identity.Models.IdentitySettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
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
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs/") || (path.HasValue ? path.Value.StartsWith("/hubs/") : false)))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // configure DI for application services
            services.AddScoped<Features.Identity.IUserService, Features.Identity.UserService>();

            services.AddHostedService<Worker>();
            services.AddSignalR();
            services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<Features.Authentication.IManifest, Features.Authentication.Manifest>();
            services.AddSingleton<Features.Authentication.IDiscovery, Features.Authentication.Discovery>();
            services.AddSingleton<Features.Authentication.IAuth, Features.Authentication.Auth>();
            services.AddSingleton<Features.Identity.IUserService, Features.Identity.UserService>();
            services.AddSingleton<IUserIdProvider, Features.Identity.ClaimUserIdProvider>();
            services.AddSingleton<Features.Pipes.IPipe, Features.Pipes.Pipe>();
            services.AddSingleton<Features.Pipes.IPipeIncomingConnection, Features.Pipes.PipeIncomingConnection>();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseSignalR(route =>
            {
                route.MapHub<ConnectHub>("/hubs/connect");
            });
        }
    }
}
