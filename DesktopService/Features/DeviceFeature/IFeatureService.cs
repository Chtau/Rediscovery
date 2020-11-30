using Rediscovery.Feature.Plugin.Interfaces;
using Rediscovery.Feature.Plugin.Models;
using Rediscovery.Shared.Base.Feature;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.DeviceFeature
{
    public interface IFeatureService
    {
        event EventHandler ProfilesChanged;
        event EventHandler SettingChanged;

        void Load();
        IDeviceFeatureImplementation GetFeatureDevice(Guid featureId);
        IClientFeatureImplementation GetFeatureClient(Guid featureId);
        List<Shared.Base.Device.FeatureDefinitionExtended> GetFeaturesManifest();
        /// <summary>
        /// get the ZIP archive path from 'ui.zip'
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        string GetFeatureUIArchivePath(Guid featureId);
        List<FeatureProfil> GetFeatureProfiles(Guid featureId);
        FeatureSetting GetFeatureSettings(Guid featureId);
        event EventHandler<ExchangeEntity<FeatureData>> RespondToClient;
        void ReceiveData(Guid featureId, ExchangeEntity<FeatureData> data);
        void StartFeature(Guid featureId, string sid);
        void StopFeature(Guid featureId, string sid);
        void ActiveDevicesChanged(Dictionary<string, string> devices);
    }
}
