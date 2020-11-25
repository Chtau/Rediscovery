using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Rediscovery.Communication.Provider.Feature
{
    public static class CommunicationFeatureProviderExtensions
    {
        public static IApplicationBuilder UseFeatureProvider(this IApplicationBuilder app)
        {
            var featureManager = app.ApplicationServices.GetRequiredService<IFeatureManager>();
            app.UseEndpoints(endpoints => endpoints.MapGrpcService<ProtoServices.FeatureExchangeService>());
            return app;
        }

        public static IServiceCollection AddFeatureProvider<TFeatureManager>(this IServiceCollection services)
            where TFeatureManager : class, IFeatureManager
        {
            services.AddGrpc();
            services.AddSingleton<IFeatureManager, TFeatureManager>();
            return services;
        }
    }
}