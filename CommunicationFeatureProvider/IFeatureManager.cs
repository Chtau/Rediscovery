using CommunicationBase.Models;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public interface IFeatureManager
    {
        event EventHandler<ExchangeEntity<FeatureData>> SendData;
        void ReceivedData(ExchangeEntity<FeatureData> deviceFeatureData);
        ExchangeEntity<FeatureState> FeatureStateChange(ExchangeEntity<FeatureState> featureStateChange);
        byte[] GetFeatureUIArchive(Guid featureId);
        List<FeatureProfil> GetFeatureProfiles(Guid featureId);
        FeatureSetting GetFeatureSettings(Guid featureId);
    }
}
