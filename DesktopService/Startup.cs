using DesktopService.Features.Authentication;
using DesktopService.Features.DeviceFeature;
using DesktopService.Features.InternalLogger;
using DesktopService.Features.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                .AddJsonFile(SharedConfigurations.DesktopService.ConfigFileNames.AppSettings, optional: false, reloadOnChange: true);

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

            services.AddControllers();

            var identitySettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.IdentityConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.IdentityConfiguration>(identitySettingsSection);
            var pipeSettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.PipeConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.PipeConfiguration>(pipeSettingsSection);
            var appSettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.AppConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.AppConfiguration> (appSettingsSection);

            var appSettings = identitySettingsSection.Get<SharedConfigurations.DesktopService.Models.IdentityConfiguration>();
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
            services.AddSingleton<Features.FeatureDefinitions.IManifest, Features.FeatureDefinitions.Manifest>();
            services.AddSingleton<Features.Authentication.IAuth, Features.Authentication.Auth>();
            services.AddSingleton<Features.Identity.IDeviceService, Features.Identity.DeviceService>();
            services.AddSingleton<IUserIdProvider, Features.Identity.ClaimUserIdProvider>();
            services.AddSingleton<IPCPipe.IPipeClient, IPCPipe.PipeClient>();
            services.AddSingleton<IPCPipe.IPipeServer, IPCPipe.PipeServer>();
            services.AddSingleton<IPCPipe.IPipeResourceProvider, IPCPipe.PipeResourceProvider>();
            services.AddSingleton<Features.RemoteResources.IRemoteResourcesIncomingConnection, Features.RemoteResources.RemoteResourcesIncomingConnection>();
            services.AddSingleton<Features.RemoteResources.IRemoteResourcesRepository, Features.RemoteResources.RemoteResourcesRepository>();
            services.AddSingleton<Features.RemoteResources.IRemoteResourcesServiceInfo, Features.RemoteResources.RemoteResourcesServiceInfo>();
            services.AddSingleton<Features.RemoteResources.IRemoteResourcesLiveLogger, Features.RemoteResources.RemoteResourcesLiveLogger>();
            services.AddSingleton<IFeatureService, FeatureService>();
            services.AddSingleton<Features.Configuration.IDistributeConfig, Features.Configuration.DistributeConfig>();
            services.AddSingleton<PluginFeature.Interfaces.IPluginLogger, Features.PluginLogger.PluginLogger>();

            services.AddSingleton<Features.Plugins.ILoadPlugins, Features.Plugins.LoadPlugins>();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddInternalLogger();
            loggerFactory.AddInternalLogger(new InternalLoggerConfiguration
            {
                LogLevel = LogLevel.Debug,
                Color = ConsoleColor.Gray
            });
            loggerFactory.AddInternalLogger(c =>
            {
                c.LogLevel = LogLevel.Information;
                c.Color = ConsoleColor.Blue;
            });
            loggerFactory.AddRemoteLogger(o =>
            {
                o.RemoteResourcesLiveLogger = app.ApplicationServices.GetRequiredService<Features.RemoteResources.IRemoteResourcesLiveLogger>();
                o.LogLevel = LogLevel.Information;
            });
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ConnectHub>("/hubs/connect");
                endpoints.MapHub<DeviceFeatureHub>("/hubs/feature");
            });
        }
    }
}
