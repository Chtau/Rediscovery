using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels.DeviceFeature;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Reflection;
using SharedCoreModels.DesktopPlugins;

namespace DesktopService.Features.DeviceFeature
{
    public class FeatureService : IFeatureService
    {
        private List<IDeviceFeatureImplementation> deviceFeatureImplementations = new List<IDeviceFeatureImplementation>();
        private readonly IHubContext<DeviceFeatureHub> _hubContext;
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;
        private readonly Features.Plugins.ILoadPlugins _loadPlugins;

        public FeatureService(IHubContext<DeviceFeatureHub> hubContext,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions,
            Features.Plugins.ILoadPlugins loadPlugins)
        {
            _hubContext = hubContext;
            _appSettings = appOptions.Value;
            _loadPlugins = loadPlugins;
            Load();
        }

        public IDeviceFeatureImplementation GetFeature(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId);
        }

        public List<SharedCoreModels.DeviceFeature.DeviceFeatureDefinition> GetFeaturesManifest()
        {
            var manifest = new List<SharedCoreModels.DeviceFeature.DeviceFeatureDefinition>();
            foreach (var item in deviceFeatureImplementations)
            {
                manifest.Add(item.GetDeviceFeatureInfo());
            }
            return manifest;
        }

        public void Load()
        {
            // TODO: load plugins here
            // TODO: client should handle multiple profiles
            IEnumerable<IDeviceFeatureImplementation> desktopPluginFeatures = _appSettings.Plugins?.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = _loadPlugins.LoadPlugin(pluginPath);
                return _loadPlugins.CreateDesktopPluginFeature(pluginAssembly);
            })?.ToList();
            if (desktopPluginFeatures?.Count() > 0)
            {
                foreach (var item in desktopPluginFeatures)
                {
                    item.SendData += (object sender, DeviceFeatureData e) =>
                    {
                        ResponseToClient(item.GetDeviceFeatureInfo().Id, e);
                    };
                    deviceFeatureImplementations.Add(item);
                }
            }
            /*var console = new DesktopFeatureConsole.DeviceFeatureConsole();
            console.SendData += (object sender, DeviceFeatureData e) =>
            {
                ResponseToClient(console.GetDeviceFeatureInfo().Id, e);
            };
            deviceFeatureImplementations.Add(console);
            foreach (var item in DesktopFeatureMediaPlayer.DeviceFeatureMediaPlayer.GetProfiles())
            {
                var mediaPlayer = new DesktopFeatureMediaPlayer.DeviceFeatureMediaPlayer(item);
                mediaPlayer.SendData += (object sender, DeviceFeatureData e) =>
                {
                    ResponseToClient(mediaPlayer.GetDeviceFeatureInfo().Id, e);
                };
                deviceFeatureImplementations.Add(mediaPlayer);
            }*/
        }

        private void ResponseToClient(Guid featureId, DeviceFeatureData data)
        {
            System.Diagnostics.Debug.Print($"Feature (id: {featureId}) response =>" + data.Data);
            _hubContext.Clients.User(data.DeviceId).SendAsync("ClientResponse", featureId, data.Data);
        }
    }
}
