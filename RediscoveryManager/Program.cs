using CommunicationAuthenticationConsumer;
using Microsoft.Extensions.Configuration;
using RediscoveryManager.Service;
using System;
using System.IO;

namespace RediscoveryManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(SharedConfigurations.RediscoveryManager.ConfigFileNames.AppSettings, optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var connectSettings = configuration.GetSection(SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration.SectionName).Get<SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration>();


            IManager manager = new Manager(null);
            var uiHandler = new UIHandler(manager);
            uiHandler.Start(args);
        }
    }
}
