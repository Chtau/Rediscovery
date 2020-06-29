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
using DesktopService.Features.Plugins;
using SharedBase.Feature;

namespace DesktopService.Features.DeviceFeature
{
    public class FeatureService : IFeatureService
    {
        private List<IDeviceFeatureImplementation> deviceFeatureImplementations = new List<IDeviceFeatureImplementation>();
        private List<IClientFeatureImplementation> clientFeatureImplementations = new List<IClientFeatureImplementation>();
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;
        private readonly Features.Plugins.ILoadPlugins _loadPlugins;
        private readonly ILogger<FeatureService> _logger;

        public event EventHandler ProfilesChanged;
        public event EventHandler SettingChanged;
        public event EventHandler<ExchangeEntity<FeatureData>> RespondToClient;

        public FeatureService(ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions,
            Features.Plugins.ILoadPlugins loadPlugins)
        {
            _logger = loggerFactory.CreateLogger<FeatureService>();
            _appSettings = appOptions.Value;
            _loadPlugins = loadPlugins;
            Load();
        }

        public IDeviceFeatureImplementation GetFeatureDevice(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId);
        }

        public IClientFeatureImplementation GetFeatureClient(Guid featureId)
        {
            return clientFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId);
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

        public List<FeatureProfil> GetFeatureProfiles(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetProfiles()?.GetFeatureProfils();
        }

        public FeatureSetting GetFeatureSettings(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetSettingsObject()?.GetFeatureSetting();
        }

        public List<SharedBase.Device.FeatureDefinitionExtended> GetFeaturesManifest()
        {
            var manifest = new List<SharedBase.Device.FeatureDefinitionExtended>();
            foreach (var item in deviceFeatureImplementations)
            {
                try
                {
                    var def = item.GetDeviceFeatureInfo()?.GetFeatureDefinitionExtended();
                    if (def != null && string.IsNullOrWhiteSpace(def.PluginDirectory))
                    {
                        def.PluginDirectory = item.PluginDirectory;
                    }
                    manifest.Add(def);
                } catch (Exception ex)
                {
                    _logger.LogError(ex, $"Could not get Feature definition of Plugin: \"{nameof(item)}\"");
                }
            }
            foreach (var item in clientFeatureImplementations)
            {
                try
                {
                    var def = item.GetDeviceFeatureInfo()?.GetFeatureDefinitionExtended();
                    if (def != null && string.IsNullOrWhiteSpace(def.PluginDirectory))
                    {
                        def.PluginDirectory = item.PluginDirectory;
                    }
                    manifest.Add(def);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Could not get Feature definition of Plugin: \"{nameof(item)}\"");
                }
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
                    item.SendData += (object sender, PluginExchangeEntity<PluginFeatureData> e) =>
                    {
                        _logger.LogTrace($"Feature (id: {item.GetDeviceFeatureInfo().Id} profile: {e.Entity.ProfileId}) response =>" + e.Entity.Data);
                        RespondToClient?.Invoke(this, e.GetExchangeEntity());
                    };
                    deviceFeatureImplementations.Add(item);
                }
            }
            IEnumerable<IClientFeatureImplementation> clientPluginFeatures = _appSettings.Plugins?.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = _loadPlugins.LoadPlugin(pluginPath);
                return _loadPlugins.CreateClientPluginFeature(pluginAssembly, Path.GetDirectoryName(pluginPath));
            })?.ToList();
            if (clientPluginFeatures?.Count() > 0)
            {
                foreach (var item in clientPluginFeatures)
                {
                    item.SendData += (object sender, PluginExchangeEntity<PluginFeatureDataClient> e) =>
                    {
                        _logger.LogTrace($"Feature (id: {item.GetDeviceFeatureInfo().Id} profile: {e.Entity.ProfileId}) response =>" + e.Entity.Data);
                        RespondToClient?.Invoke(this, e.GetExchangeEntity());
                    };
                    clientFeatureImplementations.Add(item);
                }
            }
        }

        public void ReceiveData(Guid featureId, ExchangeEntity<FeatureData> data)
        {
            if (data?.Entity?.IsClientImplementation == true)
            {
                clientFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.ReceiveData(data?.GetPluginExchangeEntityClient());
            } else
            {
                deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.ReceiveData(data?.GetPluginExchangeEntity());
            }
        }

        public void StartFeature(Guid featureId, string sid)
        {
            deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.Register(sid);
            clientFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.Register(sid);
        }

        public void StopFeature(Guid featureId, string sid)
        {
            deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.Unregister(sid);
            clientFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId)?.Unregister(sid);
        }
    }
}
