using PluginFeature.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.Plugins
{
    public interface ILoadPlugins
    {
        void LoadPaths();
        IEnumerable<IDeviceFeatureImplementation> GetDeviceFeatureImplementations();
        IEnumerable<IClientFeatureImplementation> GetClientFeatureImplementations();
        IEnumerable<string> GetMissingFeatureImplementationsInFilePaths();
    }
}
