using PluginFeature.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DesktopService.Features.Plugins
{
    public class LoadPlugins : ILoadPlugins
    {
        private readonly IPluginLogger _pluginLogger;

        public LoadPlugins(IPluginLogger pluginLogger)
        {
            _pluginLogger = pluginLogger;
        }

        public Assembly LoadPlugin(string path)
        {
            if (System.IO.File.Exists(path))
            {
                Console.WriteLine($"Loading Plugin from: {path}");
                PluginLoadContext loadContext = new PluginLoadContext(path);
                return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
            }
            return null;
        }

        public IEnumerable<IDeviceFeatureImplementation> CreateDesktopPluginFeature(Assembly assembly, string path)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IDeviceFeatureImplementation).IsAssignableFrom(type))
                {
                    IDeviceFeatureImplementation result = Activator.CreateInstance(type) as IDeviceFeatureImplementation;
                    if (result != null)
                    {
                        result.Init(path, _pluginLogger);
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IDeviceFeatureImplementation in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
