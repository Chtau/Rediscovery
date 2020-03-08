using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.FeatureDefinitions
{
    public interface IManifest
    {
        SharedCoreModels.Manifest GetManifest();
    }
}
