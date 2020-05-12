using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DALDesktopService
{
    public static class DALDesktopServiceExtensions
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, Action<DALConfiguration> configure)
        {
            var config = new DALConfiguration();
            configure(config);
            ConfigurationInstance.Configuration = config;
            services.AddSingleton<IDBContext, DBContext>();
            services.AddSingleton<Repository.IDeviceRepository, Repository.DeviceRepository>();
            services.AddSingleton<Repository.IDevicePendingAuthenticationRepository, Repository.DevicePendingAuthenticationRepository>();
            return services;
        }
    }
}
