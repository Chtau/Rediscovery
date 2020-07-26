using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PluginFeature.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DesktopService.Features.Plugins
{
    public class LoadPlugins : ILoadPlugins
    {
        public class PluginAssembly
        {
            public Assembly Assembly { get; set; }
            public string PluginPath { get; set; }
        }

        private readonly IPluginLogger _pluginLogger;
        private readonly ILogger<LoadPlugins> _logger;
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;

        private List<string> filePaths = new List<string>();
        private List<string> directoryPaths = new List<string>();
        private List<string> missingPluginImplementation = new List<string>();
        private List<PluginAssembly> pluginAssemblies = new List<PluginAssembly>();

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
                missingPluginImplementation.Clear();
                filePaths.Clear();
                directoryPaths.Clear();

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
                if (!Directory.Exists(defaultPluginDirectory))
                {
                    Directory.CreateDirectory(defaultPluginDirectory);
                }

                // zip files in default plugin directory are plugins
                var files = Directory.EnumerateFiles(defaultPluginDirectory, "*.zip");
                if (files?.Count() > 0)
                    filePaths.AddRange(files);

                foreach (var path in filePaths.Where(x => x.EndsWith(".zip")))
                {
                    try
                    {
                        using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Update))
                        {
                            archive.ExtractToDirectory(defaultPluginDirectory, true);
                        }
                        File.Delete(path);
                    } catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }
                filePaths = filePaths.Where(x => !x.EndsWith(".zip")).ToList();

                // sub directories are plugins
                var subDirs = Directory.EnumerateDirectories(defaultPluginDirectory);
                if (subDirs?.Count() > 0)
                    directoryPaths.AddRange(subDirs);

                // search for plugin assemblies in the directories

                LoadAssemblies();
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public IEnumerable<IDeviceFeatureImplementation> GetDeviceFeatureImplementations()
        {
            return pluginAssemblies.SelectMany(pluginAssembly =>
            {
                var result = CreateDesktopPluginFeature(pluginAssembly.Assembly, Path.GetDirectoryName(pluginAssembly.PluginPath));
                if (!(result?.Count() > 0))
                {
                    missingPluginImplementation.Add(pluginAssembly.PluginPath);
                }
                return result;
            })?.ToList()?.Where(x => x != null);
        }

        public IEnumerable<IClientFeatureImplementation> GetClientFeatureImplementations()
        {
            return pluginAssemblies.SelectMany(pluginAssembly =>
            {
                var result = CreateClientPluginFeature(pluginAssembly.Assembly, Path.GetDirectoryName(pluginAssembly.PluginPath));
                if (!(result?.Count() > 0))
                {
                    missingPluginImplementation.Add(pluginAssembly.PluginPath);
                }
                return result;
            })?.ToList()?.Where(x => x != null);
        }

        public IEnumerable<string> GetMissingFeatureImplementationsInFilePaths()
        {
            return filePaths.Except(missingPluginImplementation);
        }

        private void LoadAssemblies()
        {
            foreach (var pluginPath in filePaths)
            {
                try
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    if (pluginAssembly != null)
                    {
                        pluginAssemblies.Add(new PluginAssembly
                        {
                            Assembly = pluginAssembly,
                            PluginPath = pluginPath
                        });
                    }
                } catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        private Assembly LoadPlugin(string path)
        {
            if (System.IO.File.Exists(path))
            {
                PluginLoadContext loadContext = new PluginLoadContext(path);
                var result = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                if (result != null)
                    _logger.LogInformation($"Assembly successful loaded (Assembly:\"{result.GetName().Name}\" Path:\"{path}\")");
                else
                    _logger.LogCritical($"Failed to load Assembly (Path:\"{path}\")");
                return result;
            } else
            {
                _logger.LogWarning($"Request Assembly on Configuration Path:\"{path}\" does not exist.");
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
