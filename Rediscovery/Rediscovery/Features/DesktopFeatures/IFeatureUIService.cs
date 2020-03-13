using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IFeatureUIService
    {
        void SaveUI(Guid featureId, Action<bool, string> callback);

        string UIDirectory(Guid featureId);
        void GetProfil(Guid featureId, Action<bool, List<DeviceFeatureProfil>> callback);
        void GetSetting(Guid featureId, Action<bool, DeviceFeatureSetting> callback);
    }
}
