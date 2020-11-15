using Microsoft.Extensions.Configuration;
using RediscoveryManager.Service;
using System.IO;

namespace RediscoveryManager
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(SharedConfigurations.RediscoveryManager.ConfigFileNames.AppSettings, optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var jsonSetting = configuration.GetSection(SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration.SectionName).Value;
            var connectSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration>(jsonSetting); //configuration.GetSection(SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration.SectionName).Get<SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration>();
            var argCon = TryParseConnectionArguments(args);

            SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration connection = new SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration();
            if (argCon != null)
                connection = argCon;
            if (connectSettings != null)
                connection = connectSettings;
            IManager manager = new Manager();
            var uiHandler = new UIHandler(manager);
            uiHandler.Start(connection);
        }

        private static SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration TryParseConnectionArguments(string[] args)
        {
            var deviceIdentifier = Arguments.TryParseArgumentValue(args, Arguments.SetDeviceIdentifier);
            var ip = Arguments.TryParseArgumentValue(args, Arguments.SetIP);
            var portString = Arguments.TryParseArgumentValue(args, Arguments.SetPort);
            int.TryParse(portString, out int port);
            var autoConnect = Arguments.TryParseArgumentMatch(args, Arguments.Autoconnect);
            if (!string.IsNullOrWhiteSpace(ip) || port > 0 || !string.IsNullOrWhiteSpace(deviceIdentifier))
            {
                return new SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration
                {
                    AutoConnect = autoConnect,
                    DeviceIdentifier = deviceIdentifier,
                    Port = port,
                    IP = ip
                };
            }
            else
            {
                return null;
            }
        }
    }
}