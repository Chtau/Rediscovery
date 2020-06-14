using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IFeatureUIService
    {
        void SaveUI(Guid modelId, Guid featureId, Action<bool, string> callback);

        string UIDirectory(Guid featureId);
        void GetProfil(Guid modelId, Guid featureId, Action<bool, List<FeatureProfil>> callback);
        void GetSetting(Guid modelId, Guid featureId, Action<bool, FeatureSetting> callback);
    }
}
