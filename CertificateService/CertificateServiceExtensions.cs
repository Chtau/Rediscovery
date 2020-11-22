using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rediscovery.Service.Certificate
{
    public static class CertificateServiceExtensions
    {
        public static IServiceCollection AddCertificateService(this IServiceCollection services)
        {
            services.AddCertificateManager();
            services.AddSingleton<ICertificateManager, CertificateManager>();
            return services;
        }

        public static IServiceCollection AddCertificateService(this IServiceCollection services, Action<DefaultCertificateConfiguration> configure)
        {
            var config = new DefaultCertificateConfiguration();
            configure(config);
            ConfigurationInstance.Configuration = config;
            return AddCertificateService(services);
        }

        public static IApplicationBuilder UseCertificateServiceDefaults(this IApplicationBuilder app)
        {
            if (!string.IsNullOrWhiteSpace(ConfigurationInstance.Configuration?.DnsIp))
            {
                var certInstance = app.ApplicationServices.GetRequiredService<ICertificateManager>();
                certInstance.Certificate(ConfigurationInstance.Configuration.DnsIp);
            }

            return app;
        }
    }
}