using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PluginFeature.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.Plugins
{
    public class LoadPlugins : ILoadPlugins
    {
        public class PluginAssembly
        {
            public Assembly Assembly { get; set; }
            public string PluginPath { get; set; }
        }

        private const string PluginPackageExtension = ".zip";

        private readonly IPluginLogger _pluginLogger;
        private readonly ILogger<LoadPlugins> _logger;
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;
        private readonly Services.IStaticResources _staticResources;

        private List<string> filePaths = new List<string>();
        private List<string> directoryPaths = new List<string>();
        private List<string> missingPluginImplementation = new List<string>();
        private List<PluginAssembly> pluginAssemblies = new List<PluginAssembly>();

        public LoadPlugins(ILoggerFactory loggerFactory, IPluginLogger pluginLogger,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions,
            Services.IStaticResources staticResources)
        {
            _logger = loggerFactory.CreateLogger<LoadPlugins>();
            _pluginLogger = pluginLogger;
            _appSettings = appOptions.Value;
            _staticResources = staticResources;
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

                string defaultPluginDirectory = _staticResources.PluginFolderName;
                string ignorePluginBackupDirectory = Path.Combine(defaultPluginDirectory, _staticResources.PluginHiddenBackupFolderName);
                if (!Directory.Exists(defaultPluginDirectory))
                {
                    Directory.CreateDirectory(defaultPluginDirectory);
                }
                if (!Directory.Exists(ignorePluginBackupDirectory))
                {
                    var dirInfo = Directory.CreateDirectory(ignorePluginBackupDirectory);
                    dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }

                // zip files in default plugin directory are plugins
                var files = Directory.EnumerateFiles(defaultPluginDirectory, $"*{PluginPackageExtension}");
                if (files?.Count() > 0)
                    filePaths.AddRange(files);

                foreach (var path in filePaths.Where(x => x.EndsWith(PluginPackageExtension)))
                {
                    try
                    {
                        using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Update))
                        {
                            archive.ExtractToDirectory(defaultPluginDirectory, true);
                        }
                        var fName = Path.GetFileName(path);
                        var targetPath = Path.Combine(ignorePluginBackupDirectory, fName);
                        File.Copy(path, targetPath, true);
                        File.Delete(path);
                    } catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }
                filePaths = filePaths.Where(x => !x.EndsWith(PluginPackageExtension)).ToList();

                // sub directories are plugins
                var subDirs = Directory.EnumerateDirectories(defaultPluginDirectory);
                if (subDirs?.Count() > 0)
                    directoryPaths.AddRange(subDirs);
                ResolveAssembliesPathFromDirectories();

                LoadAssemblies();
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public IEnumerable<IDeviceFeatureImplementation> GetDeviceFeatureImplementations()
        {
            var ids = new List<Guid>();
            return pluginAssemblies.SelectMany(pluginAssembly =>
            {
                var result = CreateDesktopPluginFeature(pluginAssembly.Assembly, Path.GetDirectoryName(pluginAssembly.PluginPath), ids).ToList();
                if (!(result?.Count() > 0))
                {
                    missingPluginImplementation.Add(pluginAssembly.PluginPath);
                    return new List<IDeviceFeatureImplementation>();
                }
                return result;
            })?.ToList()?.Where(x => x != null);
        }

        public IEnumerable<IClientFeatureImplementation> GetClientFeatureImplementations()
        {
            var ids = new List<Guid>();
            return pluginAssemblies.SelectMany(pluginAssembly =>
            {
                var result = CreateClientPluginFeature(pluginAssembly.Assembly, Path.GetDirectoryName(pluginAssembly.PluginPath), ids).ToList();
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

        private void ResolveAssembliesPathFromDirectories()
        {
            try
            {
                // search for plugin assemblies in the directories
                foreach (var dir in directoryPaths)
                {
                    var files = Directory.EnumerateFiles(dir, "*.dll");
                    if (files?.Count() > 0)
                    {
                        foreach (var file in files)
                        {
                            Assembly pluginAssembly = LoadPlugin(file);
                            if (pluginAssembly != null)
                            {
                                var result = pluginAssembly.GetTypes().Any(x => (typeof(IDeviceFeatureImplementation).IsAssignableFrom(x) || typeof(IClientFeatureImplementation).IsAssignableFrom(x)));
                                if (result)
                                {
                                    filePaths.Add(file);
                                }
                            }
                        }
                    }
                }
                directoryPaths.Clear();
            }
            catch (Exception ex1)
            {
                _logger.LogError(ex1.ToString());
            }
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

        private IEnumerable<IDeviceFeatureImplementation> CreateDesktopPluginFeature(Assembly assembly, string path, List<Guid> alreadyAddedPlugins)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IDeviceFeatureImplementation).IsAssignableFrom(type))
                {
                    IDeviceFeatureImplementation result = Activator.CreateInstance(type) as IDeviceFeatureImplementation;
                    if (result != null)
                    {
                        var id = result.GetDeviceFeatureInfo().Id;
                        if (!alreadyAddedPlugins.Contains(id))
                        {
                            alreadyAddedPlugins.Add(id);
                            var absPath = Path.GetFullPath(path);
                            result.Init(OnGetFeatureWorkingDirectory(absPath, id), _pluginLogger);
                            yield return result;
                        }
                    }
                }
            }
        }

        private IEnumerable<IClientFeatureImplementation> CreateClientPluginFeature(Assembly assembly, string path, List<Guid> alreadyAddedPlugins)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IClientFeatureImplementation).IsAssignableFrom(type))
                {
                    IClientFeatureImplementation result = Activator.CreateInstance(type) as IClientFeatureImplementation;
                    if (result != null)
                    {
                        var id = result.GetDeviceFeatureInfo().Id;
                        if (!alreadyAddedPlugins.Contains(id))
                        {
                            alreadyAddedPlugins.Add(id);
                            var absPath = Path.GetFullPath(path);
                            result.Init(OnGetFeatureWorkingDirectory(absPath, id), _pluginLogger);
                            yield return result;
                        }
                    }
                }
            }
        }

        private string OnGetFeatureWorkingDirectory(string absPath, Guid featureId)
        {
            string path = absPath;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, featureId.ToString().Replace("-", ""));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return path;
        }
    }
}
