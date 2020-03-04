using PluginFeature.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DesktopService.Features.Plugins
{
    public interface ILoadPlugins
    {
        Assembly LoadPlugin(string path);
        IEnumerable<IDeviceFeatureImplementation> CreateDesktopPluginFeature(Assembly assembly);
    }
}
