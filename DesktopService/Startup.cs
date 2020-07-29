using CertificateService;
using CommunicationAuthenticationProvider;
using CommunicationFeatureProvider;
using CommunicationHeartbeatProvider;
using CommunicationLoggerProvider;
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
using Microsoft.Extensions.Options;
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
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var identitySettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.IdentityConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.IdentityConfiguration>(identitySettingsSection);
            var remoteResourceSettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration>(remoteResourceSettingsSection);
            var appSettingsSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.AppConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.AppConfiguration> (appSettingsSection);
            var rolesSection = Configuration.GetSection(SharedConfigurations.DesktopService.Models.RoleConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopService.Models.RoleConfiguration>(rolesSection);

            var appSettings = appSettingsSection.Get<SharedConfigurations.DesktopService.Models.AppConfiguration>();
            var identitySettings = identitySettingsSection.Get<SharedConfigurations.DesktopService.Models.IdentityConfiguration>();
            var roleSettings = rolesSection.Get<SharedConfigurations.DesktopService.Models.RoleConfiguration>();

            services.AddHostedService<Worker>();

            services.AddLogging();

            string dbPath = System.IO.Path.Combine(SharedFeatureFunctions.File.GetUserFolder(appSettings.AppDataFolder), "rediscovery.db");
            if (!string.IsNullOrWhiteSpace(appSettings.DatabasePath))
            {
                var dir = System.IO.Path.GetDirectoryName(appSettings.DatabasePath);
                if (System.IO.Directory.Exists(dir))
                    dbPath = appSettings.DatabasePath;
            }

            services.AddDAL((c) =>
            {
                c.ConnectionString = dbPath;
            });
            services.AddCertificateService();

            services.AddAuthenticationProvider<AuthenticationManager>(identitySettings.Secret, roleSettings.DeviceRoleName, roleSettings.ResourceConsumerRoleName);
            services.AddFeatureProvider<FeatureManager>();
            services.AddResourceProvider<RemoteResourcesRepository, ResourceManager>();
            services.AddHeartbeatProvider<CommunicationHeartbeatProvider.Configuration>();
            services.AddLoggerProvider<CommunicationLoggerProvider.DirectLogger>();

            services.AddSingleton<Services.IStaticResources, Services.StaticResources>();

            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<IFeatureService, FeatureService>();
            services.AddSingleton<IRoleResolver, RoleResolver>();
            services.AddSingleton<Features.Configuration.IDistributeConfig, Features.Configuration.DistributeConfig>();
            services.AddSingleton<PluginFeature.Interfaces.IPluginLogger, Features.PluginLogger.PluginLogger>();

            services.AddSingleton<Features.Plugins.ILoadPlugins, Features.Plugins.LoadPlugins>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCertificateServiceDefaults();

            var appSettings = app.ApplicationServices.GetRequiredService<IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration>>();
            var remoteLogLevel = LogLevel.None;
            if (Enum.TryParse(typeof(LogLevel), appSettings?.Value?.RemoteLogger, out object obj))
            {
                remoteLogLevel = (LogLevel)obj;
            }
            loggerFactory.AddRemoteLogger(o =>
            {
                o.LoggingModuleName = "DesktopService";
                o.LogLevel = remoteLogLevel;
                o.GetLoggerHandlerInstance = () => app.ApplicationServices.GetRequiredService<ILoggerHandler>();
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
            app.UseHeartbeatProvider();
            app.UseLoggerProvider();
            app.UseFeatureProvider();
            app.UseAuthenticationProvider();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
