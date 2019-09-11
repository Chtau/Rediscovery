using DesktopService.Features.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

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
            services.AddHostedService<Worker>();
            services.AddSignalR();
            services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<Features.Authentication.IManifest, Features.Authentication.Manifest>();
            services.AddSingleton<Features.Authentication.IDiscovery, Features.Authentication.Discovery>();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSignalR(route =>
            {
                route.MapHub<ConnectHub>("/connect");
            });
        }
    }
}
