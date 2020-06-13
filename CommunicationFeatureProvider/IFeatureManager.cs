using CommunicationBase.Models;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public interface IFeatureManager
    {
        event EventHandler<PluginExchangeEntity<PluginFeature.Models.PluginFeatureData>> SendData;
        void ReceivedData(PluginExchangeEntity<PluginFeature.Models.PluginFeatureData> deviceFeatureData);
        PluginExchangeEntity<FeatureState> FeatureStateChange(PluginExchangeEntity<FeatureState> featureStateChange);
        byte[] GetFeatureUIArchive(Guid featureId);
        List<PluginFeatureProfil> GetFeatureProfiles(Guid featureId);
        PluginFeatureSetting GetFeatureSettings(Guid featureId);
    }
}
