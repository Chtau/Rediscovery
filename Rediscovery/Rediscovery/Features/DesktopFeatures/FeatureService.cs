using AngleSharp;
using Rediscovery.Features.Connection;
using Rediscovery.Services;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.FeatureService))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class FeatureService : BaseService, IFeatureService
    {
        public event EventHandler<SharedBase.Feature.FeatureData> ReceivedData;
        public event EventHandler<List<FeatureProfil>> ReceivedProfiles;
        public event EventHandler<FeatureSetting> ReceivedSetting;
        public event EventHandler<Tuple<bool, string>> ReceivedUI;

        private IConsumer consumer => DependencyService.Get<IConsumer>();
        private IDeviceData deviceData => DependencyService.Get<IDeviceData>() ?? new DeviceData();
        private IConnectService connectService => DependencyService.Get<IConnectService>();
        private Services.IFileSystem fileSystem => DependencyService.Get<Services.IFileSystem>() ?? new Services.FileSystem();
        private IHtmlUIService htmlUIService => DependencyService.Get<IHtmlUIService>() ?? new HtmlUIService();
        private DesktopConfiguration.DesktopConfigurationModel desktopConfiguration;
        private Guid featureId;

        public FeatureService()
        {
            consumer.FeatureConsumerService.ReceiveClientData += FeatureConsumer_ReceiveClientData;
            consumer.FeatureConsumerService.ReceiveFeatureData += FeatureConsumer_ReceiveFeatureData;
            consumer.FeatureConsumerService.ReceiveFeatureStateChangeReply += FeatureConsumer_ReceiveFeatureStateChangeReply;
        }

        private void FeatureConsumer_ReceiveFeatureStateChangeReply(object sender, CommunicationBase.Models.FeatureState e)
        {
            _logger.LogTrace($"{DateTime.Now.ToShortTimeString()} Feature state changed received. (FeatureId:{e.FeatureId} State:{e.CurrentState})");
        }

        private void FeatureConsumer_ReceiveFeatureData(object sender, SharedBase.Feature.FeatureData e)
        {
            _logger.LogTrace($"{DateTime.Now.ToShortTimeString()} Feature exchange received. (FeatureId:{e.FeatureId} ProfileId:{e.ProfileId})");
            if (featureId == e.FeatureId)
            {
                ReceivedData?.Invoke(this, e);
            }
        }

        private void FeatureConsumer_ReceiveClientData(object sender, CommunicationFeatureConsumer.Models.FeatureClientData e)
        {
            if (e.FeatureId == featureId)
            {
                ReceivedProfiles?.Invoke(this, e.FeatureProfils);
                ReceivedSetting?.Invoke(this, e.FeatureSetting);

                string directory = OnArchiveDirectory(featureId);
                try
                {
                    if (System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.Delete(directory, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                try
                {
                    if (e.UIArchive != null)
                    {
                        var stream = new MemoryStream(e.UIArchive);
                        var archive = new ZipArchive(stream);
                        if (archive != null)
                        {
                            archive.ExtractToDirectory(directory);
                            Task.Run(async () =>
                            {
                                await OnInjectUIDefaults(directory);
                                ReceivedUI?.Invoke(this, new Tuple<bool, string>(true, directory));
                            });
                        }
                        else
                        {
                            _logger.LogInformation($"No UI Archive received for Feature Id:{featureId} (no valid ZipArchive from byte[])");
                            ReceivedUI?.Invoke(this, new Tuple<bool, string>(false, directory));
                        }
                    } else
                    {
                        _logger.LogInformation($"No UI Archive received for Feature Id:{featureId} (byte[] was \"null\")");
                        ReceivedUI?.Invoke(this, new Tuple<bool, string>(false, directory));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    ReceivedUI?.Invoke(this, new Tuple<bool, string>(false, directory));
                }
            }
        }

        public bool LoadFeature(DesktopConfiguration.DesktopConfigurationModel configurationModel, Guid featureId)
        {
            try
            {
                desktopConfiguration = configurationModel;
                this.featureId = featureId;
                var conData = connectService.GetData(desktopConfiguration.Id);
                if (consumer.FeatureConsumerService.Connect(desktopConfiguration.Address, conData.SSLPort, conData.PEM))
                {
                    consumer.FeatureConsumerService.FeatureClient(conData.Token, this.featureId);
                    consumer.FeatureConsumerService.StartFeatureData(conData.Token);
                    return true;
                }
                return false;
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        public void Start()
        {
            var conData = connectService.GetData(desktopConfiguration.Id);
            consumer.FeatureConsumerService.ChangeFeatureState(conData.Token, new CommunicationBase.Models.FeatureState
            {
                FeatureId = featureId,
                CurrentState = CommunicationBase.Models.FeatureState.State.Start
            });
        }

        public void Stop()
        {
            var conData = connectService.GetData(desktopConfiguration.Id);
            consumer.FeatureConsumerService.ChangeFeatureState(conData.Token, new CommunicationBase.Models.FeatureState
            {
                FeatureId = featureId,
                CurrentState = CommunicationBase.Models.FeatureState.State.Stop
            });
        }

        public void Send(string profileId, string data)
        {
            
            _logger.LogTrace($"{DateTime.Now.ToShortTimeString()} Try to send from Feature. (profileId:{profileId} data:{data})");
            consumer.FeatureConsumerService.SendFeatureData(new SharedBase.Feature.FeatureData(deviceData.GetDeviceIdentifier(), featureId, profileId, data));
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
