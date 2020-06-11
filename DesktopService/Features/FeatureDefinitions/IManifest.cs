using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.FeatureDefinitions
{
    public interface IManifest
    {
        SharedBase.Connection.Manifest GetManifest();
    }
}
