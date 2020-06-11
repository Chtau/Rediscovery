using CertificateService;
using CommunicationAuthenticationProvider;
using CommunicationFeatureProvider;
using CommunicationResourceProvider;
using DALDesktopService;
using DesktopService.Features.Authentication;
using DesktopService.Features.DeviceFeature;
using DesktopService.Features.InternalLogger;
using DesktopService.Features.Logger;
using DesktopService.Features.RemoteResources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
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
            services.AddControllers();

            var identitySettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.IdentityConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.IdentityConfiguration>(identitySettingsSection);
            var remoteResourceSettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration>(remoteResourceSettingsSection);
            var appSettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.AppConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.AppConfiguration> (appSettingsSection);

            var appSettings = appSettingsSection.Get<SharedConfigurations.DesktopService.Models.AppConfiguration>();
            var identitySettings = identitySettingsSection.Get<SharedConfigurations.DesktopService.Models.IdentityConfiguration>();
            //var key = Encoding.ASCII.GetBytes(identitySettings.Secret);
            /*services.AddAuthentication(x =>
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
            });*/

            services.AddHostedService<Worker>();

            services.AddLogging();
            services.AddDAL((c) =>
            {
                c.ConnectionString = System.IO.Path.Combine(AppFolders.GetUserFolder(appSettings.AppDataFolder), "rediscovery.db");
            });
            services.AddCertificateService(config =>
            {
                config.DnsIp = Program.HostIpAddress;
            });

            services.AddAuthenticationProvider<AuthenticationManager>(identitySettings.Secret);
            services.AddFeatureProvider<FeatureManager>();
            services.AddResourceProvider<RemoteResourcesRepository, ResourceManager>();

            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<Features.RemoteResources.IRemoteResourcesLiveLogger, Features.RemoteResources.RemoteResourcesLiveLogger>();
            services.AddSingleton<IFeatureService, FeatureService>();
            services.AddSingleton<Features.Configuration.IDistributeConfig, Features.Configuration.DistributeConfig>();
            services.AddSingleton<PluginFeature.Interfaces.IPluginLogger, Features.PluginLogger.PluginLogger>();

            services.AddSingleton<Features.Plugins.ILoadPlugins, Features.Plugins.LoadPlugins>();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCertificateServiceDefaults();
            /*loggerFactory.AddInternalLogger();
            loggerFactory.AddInternalLogger(new InternalLoggerConfiguration
            {
                LogLevel = LogLevel.Debug,
                Color = ConsoleColor.Gray
            });
            loggerFactory.AddInternalLogger(c =>
            {
                c.LogLevel = LogLevel.Information;
                c.Color = ConsoleColor.Blue;
            });*/
            loggerFactory.AddRemoteLogger(o =>
            {
                o.RemoteResourcesLiveLogger = app.ApplicationServices.GetRequiredService<Features.RemoteResources.IRemoteResourcesLiveLogger>();
                o.LogLevel = LogLevel.Trace; //TODO: make log level a configuration option
            });

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                    //.AllowCredentials();
            });


            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResourceProvider();
            app.UseFeatureProvider();
            app.UseAuthenticationProvider();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
