using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoadPlugins> _logger;

        public LoadPlugins(ILoggerFactory loggerFactory, IPluginLogger pluginLogger)
        {
            _logger = loggerFactory.CreateLogger<LoadPlugins>();
            _pluginLogger = pluginLogger;
        }

        public Assembly LoadPlugin(string path)
        {
            if (System.IO.File.Exists(path))
            {
                PluginLoadContext loadContext = new PluginLoadContext(path);
                var result = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                if (result != null)
                    _logger.LogInformation($"Plugin successful loaded (Assembly:\"{result.GetName()}\" Path:\"{path}\")");
                else
                    _logger.LogCritical($"Failed to load Plugin (Path:\"{path}\")");
                return result;
            } else
            {
                _logger.LogWarning($"Request Plugin on Configuration Path:\"{path}\" does not exist.");
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
                _pluginLogger.LogWarning(
                    $"Can't find any type which implements IDeviceFeatureImplementation in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        public IEnumerable<IClientFeatureImplementation> CreateClientPluginFeature(Assembly assembly, string path)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IClientFeatureImplementation).IsAssignableFrom(type))
                {
                    IClientFeatureImplementation result = Activator.CreateInstance(type) as IClientFeatureImplementation;
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
                _pluginLogger.LogWarning(
                    $"Can't find any type which implements IClientFeatureImplementation in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
