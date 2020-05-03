using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile(SharedConfigurations.DesktopHub.ConfigFileNames.AppSettings, optional: false, reloadOnChange: true);

            Configuration = builder.Build();
            //Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var remoteResourceSection = Configuration.GetSection(SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration.SectionName);
            services.Configure<SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration>(remoteResourceSection);

            services.AddControllersWithViews();

            services.AddHostedService<Worker>();

            services.AddSingleton<Feature.RemoteResource.IDesktopHubRemoteResourceService, Feature.RemoteResource.DesktopHubRemoteResourceService>();
            services.AddSingleton<Feature.Device.IDeviceService, Feature.Device.DeviceService>();
            services.AddSingleton<Feature.Logger.ILoggerService, Feature.Logger.LoggerService>();
            services.AddSingleton<Feature.Features.IFeatureService, Feature.Features.FeatureService>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });

            // Open the Electron-Window here
            Task.Run(async () => {
                var options = new BrowserWindowOptions
                {
                    Width = 1152,
                    Height = 864,
                    Show = false
                };
                var mainWindow = await Electron.WindowManager.CreateWindowAsync(options);
                mainWindow.OnReadyToShow += () =>
                {
                    mainWindow.Show();
                };
                mainWindow.SetTitle("Rediscovery Hub");
            });
        }
    }
}
