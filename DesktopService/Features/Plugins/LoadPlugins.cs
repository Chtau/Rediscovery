using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DesktopService.Features.Plugins
{
    // TODO: https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support

    public class LoadPlugins
    {
        public Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        public IEnumerable<SharedCoreModels.DesktopPlugins.IDesktopPluginFeatureDefinition> CreateDesktopPluginFeatureDefinition(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(SharedCoreModels.DesktopPlugins.IDesktopPluginFeatureDefinition).IsAssignableFrom(type))
                {
                    SharedCoreModels.DesktopPlugins.IDesktopPluginFeatureDefinition result = Activator.CreateInstance(type) as SharedCoreModels.DesktopPlugins.IDesktopPluginFeatureDefinition;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IDesktopPluginFeatureDefinition in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
