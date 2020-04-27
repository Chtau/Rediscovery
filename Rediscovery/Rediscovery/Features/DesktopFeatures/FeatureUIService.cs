using AngleSharp;
using PluginFeature.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.FeatureUIService))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class FeatureUIService : BaseService, IFeatureUIService
    {
        private Features.Connection.IConnect connect => DependencyService.Get<Features.Connection.IConnect>() ?? new Features.Connection.Connect();
        private Services.IFileSystem fileSystem => DependencyService.Get<Services.IFileSystem>() ?? new Services.FileSystem();
        private IHtmlUIService htmlUIService => DependencyService.Get<IHtmlUIService>() ?? new HtmlUIService();

        public void GetProfil(Guid modelId, Guid featureId, Action<bool, List<DeviceFeatureProfil>> callback)
        {
            Task.Run(async () =>
            {
                try
                {
                    var profiles = await connect.GetDeviceFeatureProfils(modelId, featureId);
                    if (profiles != null)
                    {
                        callback?.Invoke(true, profiles);
                    }
                    else
                    {
                        _logger.Message($"No Profiles received for Feature Id:{featureId}");
                        callback?.Invoke(false, null);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                finally
                {
                    callback?.Invoke(false, null);
                }
            });
        }

        public void GetSetting(Guid modelId, Guid featureId, Action<bool, DeviceFeatureSetting> callback)
        {
            Task.Run(async () =>
            {
                try
                {
                    var settings = await connect.GetDeviceFeatureSetting(modelId, featureId);
                    if (settings != null)
                    {
                        callback?.Invoke(true, settings);
                    }
                    else
                    {
                        _logger.Message($"No Settings received for Feature Id:{featureId}");
                        callback?.Invoke(false, null);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                finally
                {
                    callback?.Invoke(false, null);
                }
            });
        }

        public void SaveUI(Guid modelId, Guid featureId, Action<bool, string> callback)
        {
            Task.Run(async () =>
            {
                string directory = OnArchiveDirectory(featureId);
                try
                {
                    if (System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.Delete(directory, true);
                    }
                } catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                try
                {
                    var archive = await connect.GetUIArchive(modelId, featureId);
                    if (archive != null)
                    {
                        archive.ExtractToDirectory(directory);
                        await OnInjectUIDefaults(directory);
                        callback?.Invoke(true, directory);
                    }
                    else
                    {
                        _logger.Message($"No UI Archive received for Feature Id:{featureId}");
                        callback?.Invoke(false, directory);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                } finally
                {
                    callback?.Invoke(false, directory);
                }
            });
        }

        public string UIDirectory(Guid featureId)
        {
            return OnArchiveDirectory(featureId);
        }

        private string OnArchiveDirectory(Guid featureId)
        {
            string dir = System.IO.Path.Combine(fileSystem.FeatureHtmlUIDirectory(), featureId.ToNormalizedString());
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            return dir;
        }

        private async Task OnInjectUIDefaults(string directory)
        {
            if (System.IO.Directory.Exists(directory))
            {
                string startFile = htmlUIService.GetIndexFile(directory);
                if (!string.IsNullOrWhiteSpace(startFile))
                {
                    var config = Configuration.Default;
                    var context = BrowsingContext.New(config);
                    var source = System.IO.File.ReadAllText(startFile);
                    var document = await context.OpenAsync(req => req.Content(source));

                    var defaults = htmlUIService.GetDefaultFiles();
                    if (defaults?.Count > 0)
                    {
                        foreach (var item in defaults)
                        {
                            AngleSharp.Dom.IElement element = null;
                            string tmpFileName = null;
                            switch (item.type)
                            {
                                case HtmlUIService.DefaultFileType.JS:
                                    tmpFileName = System.IO.Path.Combine(directory, item.fileName);
                                    System.IO.File.WriteAllText(tmpFileName, item.fileContent);
                                    element = document.CreateElement("script");
                                    element.SetAttribute("src", item.fileName);
                                    document.Head.AppendChild(element);
                                    break;
                                case HtmlUIService.DefaultFileType.LINK:
                                    tmpFileName = System.IO.Path.Combine(directory, item.fileName);
                                    System.IO.File.WriteAllText(tmpFileName, item.fileContent);
                                    element = document.CreateElement("link");
                                    element.SetAttribute("href", item.fileName);
                                    document.Head.AppendChild(element);
                                    break;
                                case HtmlUIService.DefaultFileType.HTML:
                                    element = document.CreateElement("div");
                                    element.InnerHtml = item.fileContent;
                                    document.Body.AppendChild(element);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    var result = document.DocumentElement.OuterHtml;
                    System.IO.File.WriteAllText(startFile, result);
                } else
                {
                    string defaultContent = htmlUIService.NoUIHtmlDefault();
                    System.IO.File.WriteAllText(System.IO.Path.Combine(directory, "index.html"), defaultContent);
                }
            } else
            {
                throw new System.IO.DirectoryNotFoundException(directory);
            }
        }
    }
}
