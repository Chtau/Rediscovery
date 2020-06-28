using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Models
{
    public class PluginFeatureDefinitionClient : PluginFeatureDefinition
    {
        public Enums.ClientNativeResources NativeResources { get; set; } = Enums.ClientNativeResources.None;
    }
}
