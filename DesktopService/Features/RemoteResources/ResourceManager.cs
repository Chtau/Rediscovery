using CommunicationResourceProvider;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.RemoteResources
{
    public class ResourceManager : IResourceManager
    {
        private readonly ILogger<ResourceManager> _logger;

        public event EventHandler SendAllDevicesChanged;
        public event EventHandler SendDevicesChanged;
        public event EventHandler SendActiveDevicesChanged;
        public event EventHandler SendPendingDevicesChanged;
        public event EventHandler SendFeaturesChanged;

        public ResourceManager(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ResourceManager>();
        }

        public void DeleteDevice(Guid deviceId)
        {
            
        }

        public void FeatureDetailProfileDelete(Guid featureId, string profileId)
        {
            
        }

        public void FeatureDetailProfileSave(Guid featureId, FeatureProfil deviceFeatureProfil)
        {
            
        }

        public void FeatureDetailSettingSave(Guid featureId, FeatureSetting deviceFeatureSetting)
        {
            
        }

        public void ResolvePendingDevice(Guid deviceId, bool accept)
        {
            
        }

        public void UpdateDevice(SharedBase.Device.DeviceInfo deviceInfo)
        {
            
        }
    }
}
