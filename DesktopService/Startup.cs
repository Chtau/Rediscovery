using Rediscovery.Communication.Provider.Authentication;
using Rediscovery.Communication.Provider.Feature;
using Rediscovery.Communication.Provider.Heartbeat;
using Rediscovery.Communication.Provider.Logger;
using Rediscovery.Communication.Provider.Resource;
using Rediscovery.Client.App.Service.Features.Authentication;
using Rediscovery.Client.App.Service.Features.DeviceFeature;
using Rediscovery.Client.App.Service.Features.Logger;
using Rediscovery.Client.App.Service.Features.RemoteResources;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Rediscovery.Service.Certificate;
using Rediscovery.Service.DAL;

namespace Rediscovery.Client.App.Service
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile(Rediscovery.Shared.Configurations.Service.ConfigFileNames.AppSettings, optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var identitySettingsSection = Configuration.GetSection(Rediscovery.Shared.Configurations.Service.Models.IdentityConfiguration.SectionName);
            services.Configure<Rediscovery.Shared.Configurations.Service.Models.IdentityConfiguration>(identitySettingsSection);
            var remoteResourceSettingsSection = Configuration.GetSection(Rediscovery.Shared.Configurations.Service.Models.RemoteResourceConfiguration.SectionName);
            services.Configure<Rediscovery.Shared.Configurations.Service.Models.RemoteResourceConfiguration>(remoteResourceSettingsSection);
            var appSettingsSection = Configuration.GetSection(Rediscovery.Shared.Configurations.Service.Models.AppConfiguration.SectionName);
            services.Configure<Rediscovery.Shared.Configurations.Service.Models.AppConfiguration>(appSettingsSection);
            var rolesSection = Configuration.GetSection(Rediscovery.Shared.Configurations.Service.Models.RoleConfiguration.SectionName);
            services.Configure<Rediscovery.Shared.Configurations.Service.Models.RoleConfiguration>(rolesSection);

            var appSettings = appSettingsSection.Get<Rediscovery.Shared.Configurations.Service.Models.AppConfiguration>();
            var identitySettings = identitySettingsSection.Get<Rediscovery.Shared.Configurations.Service.Models.IdentityConfiguration>();
            var roleSettings = rolesSection.Get<Rediscovery.Shared.Configurations.Service.Models.RoleConfiguration>();

            services.AddHostedService<Worker>();

            services.AddLogging();

            string dbPath = System.IO.Path.Combine(Feature.Shared.Functions.File.GetUserFolder(appSettings.AppDataFolder), "rediscovery.db");
            if (!string.IsNullOrWhiteSpace(appSettings.DatabasePath))
            {
                var dir = System.IO.Path.GetDirectoryName(appSettings.DatabasePath);
                if (System.IO.Directory.Exists(dir))
                    dbPath = appSettings.DatabasePath;
            }

            services.AddDAL((c) => c.ConnectionString = dbPath);
            services.AddCertificateService();

            services.AddAuthenticationProvider<AuthenticationManager>(identitySettings.Secret, roleSettings.DeviceRoleName, roleSettings.ResourceConsumerRoleName);
            services.AddFeatureProvider<FeatureManager>();
            services.AddResourceProvider<RemoteResourcesRepository, ResourceManager>();
            services.AddHeartbeatProvider<Rediscovery.Communication.Provider.Heartbeat.Configuration>();
            services.AddLoggerProvider<Rediscovery.Communication.Provider.Logger.DirectLogger>();

            services.AddSingleton<Services.IStaticResources, Services.StaticResources>();

            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<IFeatureService, FeatureService>();
            services.AddSingleton<IRoleResolver, RoleResolver>();
            services.AddSingleton<Features.Configuration.IDistributeConfig, Features.Configuration.DistributeConfig>();
            services.AddSingleton<Feature.Plugin.Interfaces.IPluginLogger, Features.PluginLogger.PluginLogger>();

            services.AddSingleton<Features.Plugins.ILoadPlugins, Features.Plugins.LoadPlugins>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCertificateServiceDefaults();

            var appSettings = app.ApplicationServices.GetRequiredService<IOptions<Rediscovery.Shared.Configurations.Service.Models.AppConfiguration>>();
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
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}