using CommunicationBase.Models;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public interface IFeatureManager
    {
        event EventHandler<ExchangeEntity<PluginFeature.Models.DeviceFeatureData>> SendData;
        void ReceivedData(ExchangeEntity<PluginFeature.Models.DeviceFeatureData> deviceFeatureData);
        ExchangeEntity<FeatureState> FeatureStateChange(ExchangeEntity<FeatureState> featureStateChange);
        byte[] GetFeatureUIArchive(Guid featureId);
        List<DeviceFeatureProfil> GetFeatureProfiles(Guid featureId);
        DeviceFeatureSetting GetFeatureSettings(Guid featureId);
    }
}
