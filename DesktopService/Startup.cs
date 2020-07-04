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
            var roleSettings = identitySettingsSection.Get<SharedConfigurations.DesktopService.Models.RoleConfiguration>();

            services.AddHostedService<Worker>();

            services.AddLogging();
            services.AddDAL((c) =>
            {
                c.ConnectionString = System.IO.Path.Combine(AppFolders.GetUserFolder(appSettings.AppDataFolder), "rediscovery.db");
            });
            services.AddCertificateService();

            services.AddAuthenticationProvider<AuthenticationManager>(identitySettings.Secret, roleSettings.DeviceRoleName, roleSettings.ResourceConsumerRoleName);
            services.AddFeatureProvider<FeatureManager>();
            services.AddResourceProvider<RemoteResourcesRepository, ResourceManager>();

            services.AddSingleton<Services.IStaticResources, Services.StaticResources>();

            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<Features.RemoteResources.IRemoteResourcesLiveLogger, Features.RemoteResources.RemoteResourcesLiveLogger>();
            services.AddSingleton<IFeatureService, FeatureService>();
            services.AddSingleton<IRoleResolver, RoleResolver>();
            services.AddSingleton<Features.Configuration.IDistributeConfig, Features.Configuration.DistributeConfig>();
            services.AddSingleton<PluginFeature.Interfaces.IPluginLogger, Features.PluginLogger.PluginLogger>();

            services.AddSingleton<Features.Plugins.ILoadPlugins, Features.Plugins.LoadPlugins>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCertificateServiceDefaults();
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
