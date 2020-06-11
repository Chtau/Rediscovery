using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Reflection;
using PluginFeature.Interfaces;
using PluginFeature.Models;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace DesktopService.Features.DeviceFeature
{
    public class FeatureService : IFeatureService
    {
        private List<IDeviceFeatureImplementation> deviceFeatureImplementations = new List<IDeviceFeatureImplementation>();
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;
        private readonly Features.Plugins.ILoadPlugins _loadPlugins;
        private readonly ILogger<FeatureService> _logger;

        public event EventHandler ProfilesChanged;
        public event EventHandler SettingChanged;

        public FeatureService(ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions,
            Features.Plugins.ILoadPlugins loadPlugins)
        {
            _logger = loggerFactory.CreateLogger<FeatureService>();
            _appSettings = appOptions.Value;
            _loadPlugins = loadPlugins;
            Load();
        }

        public IDeviceFeatureImplementation GetFeature(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId);
        }

        /// <summary>
        /// get the ZIP archive path from 'ui.zip'
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public string GetFeatureUIArchivePath(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetUIArchivePath();
        }

        public List<DeviceFeatureProfil> GetFeatureProfiles(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetProfiles();
        }

        public DeviceFeatureSetting GetFeatureSettings(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetSettingsObject();
        }

        public List<SharedBase.Device.FeatureDefinitionExtended> GetFeaturesManifest()
        {
            var manifest = new List<SharedBase.Device.FeatureDefinitionExtended>();
            foreach (var item in deviceFeatureImplementations)
            {
                var def = item.GetDeviceFeatureInfo();
                if (string.IsNullOrWhiteSpace(def.PluginDirectory))
                {
                    def.PluginDirectory = item.PluginDirectory;
                }
                manifest.Add(def);
            }
            return manifest;
        }

        public void Load()
        {
            IEnumerable<IDeviceFeatureImplementation> desktopPluginFeatures = _appSettings.Plugins?.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = _loadPlugins.LoadPlugin(pluginPath);
                return _loadPlugins.CreateDesktopPluginFeature(pluginAssembly, Path.GetDirectoryName(pluginPath));
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
        }

        private void ResponseToClient(Guid featureId, DeviceFeatureData data)
        {
            _logger.LogTrace($"Feature (id: {featureId} profile: {data.ProfileId}) response =>" + data.Data);
            // TODO: use new impl.
            _hubContext.Clients.User(data.DeviceId).SendAsync("ClientResponse", featureId, data.ProfileId, data.Data);
        }

        /// <summary>
        /// get the ZIP archive path from 'settingui.zip'
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public string GetFeatureSettingsUIArchivePath(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetSettingsUIArchivePath();
        }

        /// <summary>
        /// get the ZIP archive path from 'profileui.zip'
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public string GetFeatureProfilesUIArchivePath(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetProfilesUIArchivePath();
        }

        public bool SaveFeatureSettings(Guid featureId, DeviceFeatureSetting deviceFeatureSetting)
        {
            var result = deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.SaveSetting(deviceFeatureSetting) ?? false;
            if (result)
                SettingChanged?.Invoke(this, EventArgs.Empty);
            return result;
        }

        public bool SaveFeatureProfile(Guid featureId, DeviceFeatureProfil deviceFeatureProfil)
        {
            var result = deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.SaveProfile(deviceFeatureProfil) ?? false;
            if (result)
                ProfilesChanged?.Invoke(this, EventArgs.Empty);
            return result;
        }

        public bool DeleteFeatureProfile(Guid featureId, string profileId)
        {
            var result = deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.DeleteProfile(profileId) ?? false;
            if (result)
                ProfilesChanged?.Invoke(this, EventArgs.Empty);
            return result;
        }
    }
}
