using Rediscovery.Communication.Provider.Resource;
using Microsoft.Extensions.Logging;
using Rediscovery.Feature.Plugin.Models;
using Rediscovery.Shared.Base.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.RemoteResources
{
    public class ResourceManager : IResourceManager
    {
        private readonly ILogger<ResourceManager> _logger;

        public event EventHandler SendAllDevicesChanged;
        public event EventHandler SendDevicesChanged;
        public event EventHandler SendActiveDevicesChanged;
        public event EventHandler SendPendingDevicesChanged;
        public event EventHandler SendFeaturesChanged;
        public event EventHandler SendHeartbeatChanged;
        public event EventHandler SendLoggerEntriesChanged;

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

        public void UpdateDevice(Shared.Base.Device.DeviceInfo deviceInfo)
        {
            
        }
    }
}
