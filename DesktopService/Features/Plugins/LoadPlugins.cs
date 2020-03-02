using SharedCoreModels.DesktopPlugins;
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
            // Navigate up to the solution root
            /*string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));*/

            string pluginLocation = path;// Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
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
