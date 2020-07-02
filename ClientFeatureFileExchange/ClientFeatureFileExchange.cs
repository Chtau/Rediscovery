using PluginFeature;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientFeatureFileExchange
{
    public class ClientFeatureFileExchange : BaseClientFeature
    {
        public override PluginFeatureDefinitionClient GetDeviceFeatureInfo()
        {
            return new PluginFeatureDefinitionClient
            {
                DisplayName = "File exchange",
                Id = new Guid("7C7BE7CA-DE13-4975-A099-C64FA1581E4A"),
                ControlIntegrationPoint = Enums.PluginIntegration.Desktop,
                FeatureIntegrationPoint = Enums.PluginIntegration.Mobile,
                MinimalControlIntegrationPoint = new PluginVersion() { Major = 0, Minor = 0 },
                MinimalFeatureIntegrationPoint = new PluginVersion() { Major = 0, Minor = 0 },
                Version = new PluginVersion() { Major = 0, Minor = 0 },
                Author = "Christoph Taucher",
                Documentation = null,
                Website = null,
                PluginDirectory = PluginDirectory,
                HasProfilConfiguration = false,
                HasSettingConfiguration = false,
                NativeResources = Enums.ClientNativeResources.OpenWithIntent,
                ClientDescription = "Allows to send Files from the Mobile device to a Desktop via the Share functions."
            };
        }

        public override void ReceiveData(PluginExchangeEntity<PluginFeatureDataClient> data)
        {
            base.ReceiveData(data);
            if (data != null && IsRegister(data.Entity.DeviceId))
            {
                if (!string.IsNullOrWhiteSpace(data.Entity.Data?.ToString()))
                {
                    var openWithIntent = Newtonsoft.Json.JsonConvert.DeserializeObject<PluginFeature.Models.ClientResources.OpenWithIntent>(data.Entity.Data);
                    if (openWithIntent.Content?.Length > 0)
                    {
                        string fileTitle = openWithIntent.Title;
                        if (string.IsNullOrWhiteSpace(fileTitle))
                            fileTitle = $"{DateTime.Now.ToShortDateString()}_{DateTime.Now.ToShortTimeString()}.tmp";
                        string file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileTitle);
                        System.IO.File.WriteAllBytes(file, openWithIntent.Content);
                        pluginLogger.LogInformation($"Save file to {file}");
                        ProcessStart(file);
                    } else if (!string.IsNullOrWhiteSpace(openWithIntent.HtmlContent))
                    {
                        string fileTitle = openWithIntent.Title;
                        if (string.IsNullOrWhiteSpace(fileTitle))
                            fileTitle = $"{DateTime.Now.ToShortDateString()}_{DateTime.Now.ToShortTimeString()}.html";
                        string file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileTitle);
                        System.IO.File.WriteAllText(file, openWithIntent.HtmlContent);
                        pluginLogger.LogInformation($"Save HTML file to {file}");
                        ProcessStart(file);
                    }
                    else if (!string.IsNullOrWhiteSpace(openWithIntent.TextContent))
                    {
                        bool isUri = Uri.IsWellFormedUriString(openWithIntent.TextContent, UriKind.RelativeOrAbsolute);
                        if (isUri)
                        {
                            pluginLogger.LogInformation($"Open TextContent as URL {openWithIntent.TextContent}");
                            ProcessStart(openWithIntent.TextContent);
                        } else
                        {
                            string fileTitle = openWithIntent.Title;
                            if (string.IsNullOrWhiteSpace(fileTitle))
                                fileTitle = $"{DateTime.Now.ToShortDateString()}_{DateTime.Now.ToShortTimeString()}.txt";
                            string file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileTitle);
                            System.IO.File.WriteAllText(file, openWithIntent.HtmlContent);
                            pluginLogger.LogInformation($"Save Text file to {file}");
                            ProcessStart(file);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(openWithIntent.Uri))
                    {
                        bool isUri = Uri.IsWellFormedUriString(openWithIntent.Uri, UriKind.RelativeOrAbsolute);
                        if (isUri)
                        {
                            pluginLogger.LogInformation($"Open Uri as URL {openWithIntent.Uri}");
                            ProcessStart(openWithIntent.Uri);
                        }
                    } else
                    {
                        pluginLogger.LogError($"[OpenWithIntent] received data message but no valid data was present. {Environment.NewLine}(Model:{data.Entity.Data})");
                    }
                }
            }
        }

        private void ProcessStart(string file)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("explorer.exe", file));
            } catch (Exception ex)
            {
                pluginLogger.LogError(ex, $"Could not start process for File:{file}");
            }
        }
    }
}
