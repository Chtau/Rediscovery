using DesktopService.Features.Authentication;
using DesktopService.Features.DeviceFeature;
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
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            var identitySettingsSection = Configuration.GetSection("IdentitySettings");
            services.Configure<Features.Identity.Models.IdentitySettings>(identitySettingsSection);
            var pipeSettingsSection = Configuration.GetSection("PipeSettings");
            services.Configure<Features.Pipes.Models.PipeSettings>(pipeSettingsSection);
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

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
            services.AddScoped<Features.Identity.IDeviceService, Features.Identity.DeviceService>();

            services.AddHostedService<Worker>();

            services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<DAL.IDBContext, DAL.DBContext>();
            services.AddSingleton<Features.Authentication.IManifest, Features.Authentication.Manifest>();
            services.AddSingleton<Features.Authentication.IDiscovery, Features.Authentication.Discovery>();
            services.AddSingleton<Features.Authentication.IAuth, Features.Authentication.Auth>();
            services.AddSingleton<Features.Identity.IDeviceService, Features.Identity.DeviceService>();
            services.AddSingleton<IUserIdProvider, Features.Identity.ClaimUserIdProvider>();
            services.AddSingleton<IPCPipe.IPipeClient, IPCPipe.PipeClient>();
            services.AddSingleton<IPCPipe.IPipeServer, IPCPipe.PipeServer>();
            services.AddSingleton<IPCPipe.IPipeResourceProvider, IPCPipe.PipeResourceProvider>();
            services.AddSingleton<Features.Pipes.IPipeIncomingConnection, Features.Pipes.PipeIncomingConnection>();
            services.AddSingleton<Features.Pipes.IPipeRepository, Features.Pipes.PipeRepository>();
            services.AddSingleton<Features.Pipes.IPipeServiceInfo, Features.Pipes.PipeServiceInfo>();
            services.AddSingleton<IFeatureService, FeatureService>();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ConnectHub>("/hubs/connect");
                endpoints.MapHub<DeviceFeatureHub>("/hubs/feature");
            });
        }
    }
}
