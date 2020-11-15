using DesktopService.Features.Plugins;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PluginFeature.Interfaces;
using PluginFeature.Models;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DesktopService.Features.DeviceFeature
{
    public class FeatureService : IFeatureService
    {
        private readonly List<IDeviceFeatureImplementation> _deviceFeatureImplementations = new List<IDeviceFeatureImplementation>();
        private readonly List<IClientFeatureImplementation> _clientFeatureImplementations = new List<IClientFeatureImplementation>();
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;
        private readonly Features.Plugins.ILoadPlugins _loadPlugins;
        private readonly ILogger<FeatureService> _logger;
        private readonly Dictionary<Guid, IPCPipe.IPipeExchange> _pipeExchanges = new Dictionary<Guid, IPCPipe.IPipeExchange>();

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

        public void ActiveDevicesChanged(Dictionary<string, string> devices)
        {
            try
            {
                if (_clientFeatureImplementations?.Count > 0)
                {
                    foreach (var clientFeature in _clientFeatureImplementations)
                    {
                        clientFeature.SetDevices(devices);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public IDeviceFeatureImplementation GetFeatureDevice(Guid featureId)
        {
            return _deviceFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId);
        }

        public IClientFeatureImplementation GetFeatureClient(Guid featureId)
        {
            return _clientFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId);
        }

        /// <summary>
        /// get the ZIP archive path from 'ui.zip'
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public string GetFeatureUIArchivePath(Guid featureId)
        {
            return _deviceFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetUIArchivePath();
        }

        public List<FeatureProfil> GetFeatureProfiles(Guid featureId)
        {
            return _deviceFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetProfiles()?.GetFeatureProfils();
        }

        public FeatureSetting GetFeatureSettings(Guid featureId)
        {
            return _deviceFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.GetSettingsObject()?.GetFeatureSetting();
        }

        public List<SharedBase.Device.FeatureDefinitionExtended> GetFeaturesManifest()
        {
            var manifest = new List<SharedBase.Device.FeatureDefinitionExtended>();
            foreach (var item in _deviceFeatureImplementations)
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
            foreach (var item in _clientFeatureImplementations)
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
            _deviceFeatureImplementations.Clear();
            _clientFeatureImplementations.Clear();
            _pipeExchanges.Clear();

            _loadPlugins.LoadPaths();
            IEnumerable<IDeviceFeatureImplementation> desktopPluginFeatures = _loadPlugins.GetDeviceFeatureImplementations();
            IEnumerable<IClientFeatureImplementation> clientPluginFeatures = _loadPlugins.GetClientFeatureImplementations();

            var missing = _loadPlugins.GetMissingFeatureImplementationsInFilePaths();
            if (missing?.Count() > 0)
            {
                foreach (var missingImpl in missing)
                {
                    _logger.LogWarning($"Could not find a feature implementation in the configuration path:{missingImpl}");
                }
            }

            if (desktopPluginFeatures?.Count() > 0)
            {
                _logger.LogInformation($"Loaded {desktopPluginFeatures.Count()} feature Desktop Plugins");
                foreach (var item in desktopPluginFeatures)
                {
                    Guid featureId = item.GetDeviceFeatureInfo().Id;
                    item.SendData += (object sender, PluginExchangeEntity<PluginFeatureData> e) =>
                    {
                        _logger.LogTrace($"Feature (id: {featureId} profile: {e.Entity.ProfileId}) response =>" + e.Entity.Data);
                        RespondToClient?.Invoke(this, e.GetExchangeEntity());
                    };
                    _deviceFeatureImplementations.Add(item);
                    OnAddFeatureDesktopUICommunication(item, featureId);
                }
            }
            else
            {
                _logger.LogInformation($"No feature Desktop Plugins loaded");
            }

            if (clientPluginFeatures?.Count() > 0)
            {
                _logger.LogInformation($"Loaded {clientPluginFeatures.Count()} feature Client Plugins");
                foreach (var item in clientPluginFeatures)
                {
                    Guid featureId = item.GetDeviceFeatureInfo().Id;
                    item.SendData += (object sender, PluginExchangeEntity<PluginFeatureDataClient> e) =>
                    {
                        _logger.LogTrace($"Feature (id: {featureId} profile: {e.Entity.ProfileId}) response =>" + e.Entity.Data);
                        RespondToClient?.Invoke(this, e.GetExchangeEntity());
                    };
                    _clientFeatureImplementations.Add(item);
                    OnAddFeatureDesktopUICommunication(item, featureId);
                }
            }
            else
            {
                _logger.LogInformation($"No feature Client Plugins loaded");
            }
        }

        private void OnAddFeatureDesktopUICommunication<T>(T item, Guid featureId)
        {
            if (item is IFeatureDesktopUICommunicaton desktopUICommunicaton)
            {
                var pipeExchange = new IPCPipe.PipeExchange();
                pipeExchange.Init(featureId.ToString(), "out", "in");
                pipeExchange.DataReceived += (obj, args) =>
                {
                    try
                    {
                        desktopUICommunicaton.ReceivedChangesFromUI(args);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                };
                desktopUICommunicaton.SendChangesToUI += (obj, args) =>
                {
                    try
                    {
                        pipeExchange.Send(args);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                };
                _pipeExchanges.Add(featureId, pipeExchange);
            }
        }

        public void ReceiveData(Guid featureId, ExchangeEntity<FeatureData> data)
        {
            if (data?.Entity?.IsClientImplementation == true)
            {
                _clientFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.ReceiveData(data?.GetPluginExchangeEntityClient());
            }
            else
            {
                _deviceFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.ReceiveData(data?.GetPluginExchangeEntity());
            }
        }

        public void StartFeature(Guid featureId, string sid)
        {
            _deviceFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.Register(sid);
            _clientFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.Register(sid);
        }

        public void StopFeature(Guid featureId, string sid)
        {
            _deviceFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.Unregister(sid);
            _clientFeatureImplementations.Find(x => x.GetDeviceFeatureInfo().Id == featureId)?.Unregister(sid);
        }
    }
}