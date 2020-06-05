using CommunicationBase.Models;
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
    }
}
