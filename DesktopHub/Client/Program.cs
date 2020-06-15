using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DesktopHub.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddSingleton<CommunicationAuthenticationConsumer.IGreetingConsumerService, CommunicationAuthenticationConsumer.GreetingConsumerService>(x => new CommunicationAuthenticationConsumer.GreetingConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance));
            builder.Services.AddSingleton<CommunicationAuthenticationConsumer.IAuthenticationConsumerService, CommunicationAuthenticationConsumer.AuthenticationConsumerService>(x => new CommunicationAuthenticationConsumer.AuthenticationConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance));
            builder.Services.AddSingleton<CommunicationResourceConsumer.IResourceConsumerService, CommunicationResourceConsumer.ResourceConsumerService>(x => new CommunicationResourceConsumer.ResourceConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance));
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
