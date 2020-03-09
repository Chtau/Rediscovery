using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IFeatureUIService
    {
        void SaveUI(Guid featureId);

        string UIDirectory(Guid featureId);
    }
}
