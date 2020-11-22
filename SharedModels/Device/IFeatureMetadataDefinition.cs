using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Device
{
    public interface IFeatureMetadataDefinition
    {
        string Author { get; set; }

        string Documentation { get; set; }

        string Website { get; set; }

        string PluginDirectory { get; set; }
    }
}
