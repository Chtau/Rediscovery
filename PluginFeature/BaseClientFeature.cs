using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature
{
    public class BaseClientFeature : BaseFeature<PluginFeatureDataClient, PluginFeatureDefinitionClient>, IClientFeatureImplementation
    {
    }
}
