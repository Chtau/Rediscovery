using PluginFeature.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DesktopService.Features.Plugins
{
    // TODO: https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support

    /*<ItemGroup>
        <ProjectReference Include = "..\PluginBase\PluginBase.csproj" >
            < Private > false </ Private >
            < ExcludeAssets > runtime </ ExcludeAssets >
        </ ProjectReference >
    </ItemGroup >*/

    public class LoadPlugins : ILoadPlugins
    {
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

        public IEnumerable<IDeviceFeatureImplementation> CreateDesktopPluginFeature(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IDeviceFeatureImplementation).IsAssignableFrom(type))
                {
                    IDeviceFeatureImplementation result = Activator.CreateInstance(type) as IDeviceFeatureImplementation;
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
                    $"Can't find any type which implements IDeviceFeatureImplementation in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
