using Microsoft.Extensions.Configuration;
using System.IO;

namespace Rediscovery.Client.App.Manager.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(Shared.Configurations.Manager.ConfigFileNames.AppSettings, optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var jsonSetting = configuration.GetSection(Shared.Configurations.Manager.Models.ConnectionConfiguration.SectionName).Value;
            var connectSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<Shared.Configurations.Manager.Models.ConnectionConfiguration>(jsonSetting); //configuration.GetSection(SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration.SectionName).Get<SharedConfigurations.RediscoveryManager.Models.ConnectionConfiguration>();
            var argCon = TryParseConnectionArguments(args);

            Shared.Configurations.Manager.Models.ConnectionConfiguration connection = new Shared.Configurations.Manager.Models.ConnectionConfiguration();
            if (argCon != null)
                connection = argCon;
            if (connectSettings != null)
                connection = connectSettings;
            IManager manager = new Manager();
            var uiHandler = new UIHandler(manager);
            uiHandler.Start(connection);
        }

        private static Shared.Configurations.Manager.Models.ConnectionConfiguration TryParseConnectionArguments(string[] args)
        {
            var deviceIdentifier = Arguments.TryParseArgumentValue(args, Arguments.SetDeviceIdentifier);
            var ip = Arguments.TryParseArgumentValue(args, Arguments.SetIP);
            var portString = Arguments.TryParseArgumentValue(args, Arguments.SetPort);
            int.TryParse(portString, out int port);
            var autoConnect = Arguments.TryParseArgumentMatch(args, Arguments.Autoconnect);
            if (!string.IsNullOrWhiteSpace(ip) || port > 0 || !string.IsNullOrWhiteSpace(deviceIdentifier))
            {
                return new Shared.Configurations.Manager.Models.ConnectionConfiguration
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