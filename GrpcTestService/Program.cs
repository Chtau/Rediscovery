using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrpcTestService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.ListenAnyIP(5001, (lo) =>
                        {
                            lo.UseConnectionLogging();
                            lo.UseHttps(GetX509Certificate2());
                            lo.UseConnectionLogging();
                        });
                    })
                    .UseStartup<Startup>();
                });

        public static X509Certificate2 GetX509Certificate2()
        {
            var cert = new X509Certificate2(Path.Combine(@"C:\DEV\TMP", "development.pfx"), "1234");
            Console.WriteLine(cert.FriendlyName + " Issuer:" + cert.Issuer + " Thumbprint:" + cert.Thumbprint);
            return cert;
        }
    }
}
