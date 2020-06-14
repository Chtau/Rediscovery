using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IFeatureService
    {
        event EventHandler<SharedBase.Feature.FeatureData> ReceivedData;
        event EventHandler<List<FeatureProfil>> ReceivedProfiles;
        event EventHandler<FeatureSetting> ReceivedSetting;
        event EventHandler<Tuple<bool, string>> ReceivedUI;
        string UIDirectory(Guid featureId);
        bool LoadFeature(DesktopConfiguration.DesktopConfigurationModel configurationModel, Guid featureId);
        void Start();
        void Stop();
        void Send(string profileId, string data);
    }
}
