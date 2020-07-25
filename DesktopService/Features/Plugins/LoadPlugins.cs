using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;

        private List<string> filePaths = new List<string>();
        private List<string> directoryPaths = new List<string>();

        public LoadPlugins(ILoggerFactory loggerFactory, IPluginLogger pluginLogger,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions)
        {
            _logger = loggerFactory.CreateLogger<LoadPlugins>();
            _pluginLogger = pluginLogger;
            _appSettings = appOptions.Value;
        }

        public void LoadPaths()
        {
            try
            {
                if (_appSettings.Plugins?.Count() > 0)
                {
                    foreach (var pluginPath in _appSettings.Plugins)
                    {
                        if (Directory.Exists(pluginPath))
                            directoryPaths.Add(pluginPath);
                        else
                        {
                            if (File.Exists(pluginPath))
                                filePaths.Add(pluginPath);
                        }
                    }
                }

                string defaultPluginDirectory = "plugins";
                if (Directory.Exists(defaultPluginDirectory))
                {
                    // sub directories are plugins
                    var subDirs = Directory.EnumerateDirectories(defaultPluginDirectory);
                    if (subDirs?.Count() > 0)
                        directoryPaths.AddRange(subDirs);
                    // zip files in default plugin directory are plugins
                    var files = Directory.EnumerateFiles(defaultPluginDirectory, "*.zip");
                    if (files?.Count() > 0)
                        filePaths.AddRange(files);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public IEnumerable<IDeviceFeatureImplementation> GetDeviceFeatureImplementations()
        {
            return filePaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                if (pluginAssembly != null)
                {
                    var result = CreateDesktopPluginFeature(pluginAssembly, Path.GetDirectoryName(pluginPath));
                    if (!(result?.Count() > 0))
                    {
                        //missingPluginImplementation.Add(pluginPath);
                    }
                    return result;
                }
                else
                    return new List<IDeviceFeatureImplementation>();
            })?.ToList()?.Where(x => x != null);
        }

        public IEnumerable<IClientFeatureImplementation> GetClientFeatureImplementations()
        {
            return filePaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                if (pluginAssembly != null)
                {
                    var result = CreateClientPluginFeature(pluginAssembly, Path.GetDirectoryName(pluginPath));
                    if (!(result?.Count() > 0))
                    {
                        //missingPluginImplementation.Add(pluginPath);
                    }
                    return result;
                }
                else
                    return new List<IClientFeatureImplementation>();
            })?.ToList()?.Where(x => x != null);
        }

        private Assembly LoadPlugin(string path)
        {
            if (System.IO.File.Exists(path))
            {
                PluginLoadContext loadContext = new PluginLoadContext(path);
                var result = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                if (result != null)
                    _logger.LogInformation($"Plugin successful loaded (Assembly:\"{result.GetName().Name}\" Path:\"{path}\")");
                else
                    _logger.LogCritical($"Failed to load Plugin (Path:\"{path}\")");
                return result;
            } else
            {
                _logger.LogWarning($"Request Plugin on Configuration Path:\"{path}\" does not exist.");
            }
            return null;
        }

        private IEnumerable<IDeviceFeatureImplementation> CreateDesktopPluginFeature(Assembly assembly, string path)
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
                /*string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                _pluginLogger.LogWarning(
                    $"Can't find any type which implements IDeviceFeatureImplementation in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");*/
            }
        }

        private IEnumerable<IClientFeatureImplementation> CreateClientPluginFeature(Assembly assembly, string path)
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
                /*string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                _pluginLogger.LogWarning(
                    $"Can't find any type which implements IClientFeatureImplementation in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");*/
            }
        }
    }
}
