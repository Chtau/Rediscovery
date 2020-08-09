using PluginFeature;
using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ClientFeatureFileExchange
{
    public class ClientFeatureFileExchange : BaseClientFeature, IFeatureDesktopUICommunicaton
    {
        public event EventHandler<string> SendChangesToUI;

        public override void Init(string pluginDirectory, IPluginLogger pluginLogger)
        {
            base.Init(pluginDirectory, pluginLogger);
            try
            {
                var config = Configuration.GetConfigurations(ConfigurationPath());
            } catch (Exception ex)
            {
                pluginLogger.LogError(ex.ToString());
            }
        }

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
                ClientDescription = "Allows to send Files from the Mobile device to a Desktop via the Share functions.",
                DesktopExecutable = "ClientFeatureFileExchangeUI"
            };
        }

        public override void ReceiveData(PluginExchangeEntity<PluginFeatureDataClient> data)
        {
            base.ReceiveData(data);
            if (data != null && IsRegister(data.Entity.DeviceId))
            {
                try
                {
                    var config = Configuration.GetConfigurations(ConfigurationPath());
                    if (!string.IsNullOrWhiteSpace(data.Entity.Data?.ToString()))
                    {
                        var openWithIntent = Newtonsoft.Json.JsonConvert.DeserializeObject<PluginFeature.Models.ClientResources.OpenWithIntent>(data.Entity.Data);
                        if (openWithIntent.Content?.Length > 0)
                        {
                            string file = SaveFileBytes(config.WorkingFolder, openWithIntent.Title, openWithIntent.Content, config.FallbackFileExtensionContent);
                            pluginLogger.LogInformation($"Save file to {file}");
                            ProcessStart(config.StartProcessName, file);
                            var stringData = $"file;{file}";
                            SendChangesToUI?.Invoke(this, stringData);
                        }
                        else if (!string.IsNullOrWhiteSpace(openWithIntent.HtmlContent))
                        {
                            string file = SaveFileText(config.WorkingFolder, openWithIntent.Title, openWithIntent.HtmlContent, config.FallbackFileExtensionHtml);
                            pluginLogger.LogInformation($"Save HTML file to {file}");
                            ProcessStart(config.StartProcessName, file);
                            var stringData = $"file;{file}";
                            SendChangesToUI?.Invoke(this, stringData);
                        }
                        else if (!string.IsNullOrWhiteSpace(openWithIntent.TextContent))
                        {
                            bool isUri = Uri.IsWellFormedUriString(openWithIntent.TextContent, UriKind.RelativeOrAbsolute);
                            if (isUri)
                            {
                                pluginLogger.LogInformation($"Open TextContent as URL {openWithIntent.TextContent}");
                                ProcessStart(config.StartProcessName, openWithIntent.TextContent);
                                SendChangesToUI?.Invoke(this, $"New Text Content Url:{openWithIntent.TextContent}");
                                var stringData = $"text;{openWithIntent.TextContent}";
                                SendChangesToUI?.Invoke(this, stringData);
                            }
                            else
                            {
                                string file = SaveFileText(config.WorkingFolder, openWithIntent.Title, openWithIntent.TextContent, config.FallbackFileExtensionText);
                                pluginLogger.LogInformation($"Save Text file to {file}");
                                ProcessStart(config.StartProcessName, file);
                                var stringData = $"file;{file}";
                                SendChangesToUI?.Invoke(this, stringData);
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(openWithIntent.Uri))
                        {
                            bool isUri = Uri.IsWellFormedUriString(openWithIntent.Uri, UriKind.RelativeOrAbsolute);
                            if (isUri)
                            {
                                pluginLogger.LogInformation($"Open Uri as URL {openWithIntent.Uri}");
                                ProcessStart(config.StartProcessName, openWithIntent.Uri);
                                var stringData = $"url;{openWithIntent.Uri}";
                                SendChangesToUI?.Invoke(this, stringData);
                            }
                        }
                        else
                        {
                            pluginLogger.LogError($"[OpenWithIntent] received data message but no valid data was present. {Environment.NewLine}(Model:{data.Entity.Data})");
                        }
                    }
                }
                catch (Exception ex)
                {
                    pluginLogger.LogError(ex.ToString());
                }
            }
        }

        private string SaveFileText(string directory, string title, string content, string fallbackExtension)
        {
            string file = "";
            try
            {
                string fileTitle = title;
                if (string.IsNullOrWhiteSpace(fileTitle))
                    fileTitle = $"{DateTime.Now.ToShortDateString()}_{DateTime.Now.ToShortTimeString()}.{fallbackExtension}";
                file = System.IO.Path.Combine(directory, fileTitle);
                System.IO.File.WriteAllText(file, content);
            }
            catch (Exception ex)
            {
                pluginLogger.LogError(ex, $"Could not write File:{file}");
            }
            return file;
        }

        private string SaveFileBytes(string directory, string title, byte[] content, string fallbackExtension)
        {
            string file = "";
            try
            {
                string fileTitle = title;
                if (string.IsNullOrWhiteSpace(fileTitle))
                    fileTitle = $"{DateTime.Now.ToShortDateString()}_{DateTime.Now.ToShortTimeString()}.{fallbackExtension}";
                file = System.IO.Path.Combine(directory, fileTitle);
                System.IO.File.WriteAllBytes(file, content);
            }
            catch (Exception ex)
            {
                pluginLogger.LogError(ex, $"Could not write File:{file}");
            }
            return file;
        }

        private void ProcessStart(string startProcess, string file)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(startProcess, file));
            } catch (Exception ex)
            {
                pluginLogger.LogError(ex, $"Could not start process for File:{file}");
            }
        }

        private string ConfigurationPath()
        {
            return System.IO.Path.Combine(PluginDirectory, "config.json");
        }

        public void ReceivedChangesFromUI(string data)
        {
            System.Diagnostics.Debug.Print($"UI send data: {data}");
            try
            {
                if (!string.IsNullOrWhiteSpace(data) && data.Contains(";"))
                {
                    string[] comData = data.Split(";");
                    string command = comData[0].ToLower();
                    string content = comData[1];

                    var config = Configuration.GetConfigurations(ConfigurationPath());

                    switch (command)
                    {
                        case "sendfiles":
                            // TODO: we need a selection to a specific device
                            // TODO: should we select a folder on the selected device already or should we use a default folder on the device

                            var filePaths = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(content);
                            if (filePaths?.Count > 0)
                            {
                                var archiveName = $"{GetDeviceFeatureInfo().Id.ToString().Replace("-", "")}_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                                var sendDir = Path.Combine(PluginDirectory, archiveName);
                                if (!Directory.Exists(sendDir))
                                    Directory.CreateDirectory(sendDir);
                                var filesToUse = new List<string>();
                                foreach (var file in filePaths)
                                {
                                    if (System.IO.File.Exists(file))
                                    {
                                        string fileName = Path.GetFileName(file);
                                        string targetFile = Path.Combine(sendDir, fileName);
                                        File.Copy(file, targetFile, true);
                                    }
                                }
                                if (System.IO.Directory.Exists(sendDir))
                                {
                                    if (Directory.GetFiles(sendDir).Length > 0)
                                    {
                                        string archivePath = Path.Combine(PluginDirectory, archiveName + ".zip");
                                        if (File.Exists(archivePath))
                                            File.Delete(archivePath);
                                        ZipFile.CreateFromDirectory(sendDir, archivePath);

                                        var deviceId = ActiveDevices.First().Key;
                                        var archiveContent = Convert.ToBase64String(File.ReadAllBytes(archivePath));
                                        var featureData = new PluginFeatureDataClient(deviceId, GetDeviceFeatureInfo().Id, null, archiveContent, Enums.ClientNativeResources.FileTransfer);
                                        // TODO: before the service can send a message to the client the client must start to accept messages
                                        OnSendData(this, new PluginExchangeEntity<PluginFeatureDataClient>
                                        {
                                            Sid = deviceId,
                                            Entity = featureData
                                        });
                                    }
                                }
                            }
                            break;
                        case "open":
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                if (System.IO.File.Exists(content))
                                {
                                    ProcessStart(config.StartProcessName, content);
                                } else
                                {
                                    pluginLogger.LogWarning($"Received \"open\" File command from UI but the file no longer exists (File:{content}).");
                                }
                            } else
                            {
                                pluginLogger.LogWarning($"Received \"open\" File command from UI but content which should be file path is empty.");
                            }
                            break;
                        case "delete":
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                if (System.IO.File.Exists(content))
                                {
                                    File.Delete(content);
                                }
                                else
                                {
                                    pluginLogger.LogWarning($"Received \"delete\" File command from UI but the file no longer exists (File:{content}).");
                                }
                            }
                            else
                            {
                                pluginLogger.LogWarning($"Received \"delete\" File command from UI but content which should be file path is empty.");
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                pluginLogger.LogError(ex.ToString());
            }
        }
    }
}
