using DesktopFeatureMediaPlayer.Models;
using PluginFeature;
using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DesktopFeatureMediaPlayer
{
    public class DeviceFeatureMediaPlayer : BaseDeviceFeature
    {
        private List<MediaPlayerController> controllers = new List<MediaPlayerController>();
        private DateTime updateTimer = DateTime.Now;

        public DeviceFeatureMediaPlayer()
        {
            
        }

        public override void Init(string pluginDirectory, IPluginLogger pluginLogger)
        {
            base.Init(pluginDirectory, pluginLogger);
            foreach (var profile in MediaPlayerDefaultProfiles.GetProfileConfigurations(ProfileConfigurationPath()))
            {
                var controller = new MediaPlayerController(profile);
                controller.UpdateProcess += Controller_UpdateProcess;
                controllers.Add(controller);
            }
            if (controllers?.Count > 0)
            {
                foreach (var item in controllers)
                {
                    item.InitLogger(pluginLogger);
                }
            }
        }

        private void Controller_UpdateProcess(object sender, EventArgs e)
        {
            var newDate = DateTime.Now;
            if ((newDate - updateTimer).TotalSeconds >= 1)
            {
                updateTimer = newDate;
                foreach (var profile in MediaPlayerDefaultProfiles.GetProfileConfigurations(ProfileConfigurationPath()))
                {
                    var controller = OnGetController(profile.Id);
                    foreach (var deviceId in RegisteredDevices)
                    {
                        var dataObj = new Models.MediaPlayerStateData
                        {
                            ProcessRunning = controller.ProcessRunning,
                            Title = controller.CurrentTitle,
                            Artist = null,
                            Info = null
                        };
                        var data = new DeviceFeatureData(deviceId, GetDeviceFeatureInfo().Id, profile.Id.ToString(), dataObj);
                        OnSendData(this, data);
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override DeviceFeatureDefinition GetDeviceFeatureInfo()
        {
            return new DeviceFeatureDefinition
            {
                DisplayName = "Mediaplayer",
                Id = new Guid("D5B218BC-8F36-4100-9262-71155265DAD7"),
                ControlIntegrationPoint = IntegrationPoint.Mobile,
                FeatureIntegrationPoint = IntegrationPoint.Desktop,
                ControlIntegration = ControlIntegrationType.MediaPlayer,
                MinControlIntegrationPoint = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                Version = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                Author = "Christoph Taucher",
                Documentation = null,
                HasProfiles = true,
                HasSettings = false,
                Url = null
            };
        }

        public override void ReceiveData(DeviceFeatureData data)
        {
            base.ReceiveData(data);
            if (data != null && IsRegister(data.DeviceId))
            {
                if (!string.IsNullOrWhiteSpace(data.Data?.ToString()))
                {
                    var commandModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.CommandModel>(data.Data?.ToString());
                    if (commandModel != null)
                    {
                        OnHandleCommand(data.ProfileId, commandModel);
                    }
                    else
                    {
                        pluginLogger?.LogCritical("MediaPlayer: Unknown object from Data received");
                    }
                }
            }
        }

        private List<DeviceFeatureProfil> OnGetDeviceFeatureProfiles()
        {
            var profiles = new List<DeviceFeatureProfil>();
            var pro = MediaPlayerDefaultProfiles.GetProfileConfigurations(ProfileConfigurationPath());
            if (pro?.Count > 0)
            {
                foreach (var item in pro)
                {
                    profiles.Add(new DeviceFeatureProfil(item.Id.ToString(), item.DisplayName, Newtonsoft.Json.JsonConvert.SerializeObject(item)));
                }
            }
            return profiles;
        }

        private void OnHandleCommand(string profileId, Models.CommandModel commandModel)
        {
            var controller = OnGetController(new Guid(profileId));
            if (controller != null)
            {
                if (controller.ProcessRunning || !string.IsNullOrWhiteSpace(controller.ProfileConfiguration.ApplicationPath))
                {
                    if (!controller.ProcessRunning)
                    {
                        if (System.IO.File.Exists(controller.ProfileConfiguration.ApplicationPath))
                        {
                            if (!controller.StartProcess())
                            {
                                pluginLogger?.LogCritical($"MediaPlayer: Could not start process (Id:{profileId})");
                                return;
                            }
                        }
                        else
                        {
                            pluginLogger?.LogCritical($"MediaPlayer: Can't start Process. ApplicationPath is invalid (Id:{profileId})");
                            return;
                        }
                    }
                    controller.ExecuteCommand((CommandConfiguration.CommandTypes)commandModel.CommandIndex);
                }
                else
                {
                    pluginLogger?.LogCritical($"MediaPlayer: Process not running and no ApplicationPath to start it (Id:{profileId})");
                }
            } else
            {
                pluginLogger?.LogCritical($"MediaPlayer: No Controller for Profile (Id:{profileId})");
            }
        }

        private MediaPlayerController OnGetController(Guid profileId)
        {
            return controllers.FirstOrDefault(x => x.ProfileConfiguration.Id == profileId);
        }

        public override void Register(string deviceId)
        {
            base.Register(deviceId);
            foreach (var profile in MediaPlayerDefaultProfiles.GetProfileConfigurations(ProfileConfigurationPath()))
            {
                var controller = OnGetController(profile.Id);
                controller.InitWatcher();
            }
        }

        public override void Unregister(string deviceId)
        {
            base.Unregister(deviceId);
            foreach (var profile in MediaPlayerDefaultProfiles.GetProfileConfigurations(ProfileConfigurationPath()))
            {
                var controller = OnGetController(profile.Id);
                controller.Stop();
            }
        }

        public override List<DeviceFeatureProfil> GetProfiles()
        {
            return OnGetDeviceFeatureProfiles();
        }

        public override DeviceFeatureSetting GetSettingsObject()
        {
            return null;
        }

        private string ProfileConfigurationPath()
        {
            return System.IO.Path.Combine(PluginDirectory, "profiles.json");
        }

        public override bool SaveProfile(DeviceFeatureProfil deviceFeatureProfil)
        {
            var profiles = MediaPlayerDefaultProfiles.GetProfileConfigurations(ProfileConfigurationPath());
            var id = new Guid(deviceFeatureProfil.Id);
            var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<ProfileConfiguration>(deviceFeatureProfil.ProfileData?.ToString());
            if (profile == null)
            {
                pluginLogger.LogCritical("Try to save profile but ProfileData could not be cast to the Object [ProfileConfiguration]");
                return false;
            }
            var index = profiles.FindIndex(x => x.Id == id);
            if (index != -1)
            {
                profiles[index].ApplicationPath = profile.ApplicationPath;
                profiles[index].CommandAvailable = profile.CommandAvailable;
                profiles[index].CommandConfiguration = profile.CommandConfiguration;
                profiles[index].DisplayName = profile.DisplayName;
                profiles[index].Id = profile.Id;
                profiles[index].ProcessName = profile.ProcessName;
            } else
            {
                profiles.Add(profile);
            }
            MediaPlayerDefaultProfiles.SaveProfileConfigurations(ProfileConfigurationPath(), profiles);
            return true;
        }

        public override bool DeleteProfile(string profileId)
        {
            var profiles = MediaPlayerDefaultProfiles.GetProfileConfigurations(ProfileConfigurationPath());
            var id = new Guid(profileId);
            var index = profiles.FindIndex(x => x.Id == id);
            if (index != -1)
            {
                profiles.RemoveAt(index);
                MediaPlayerDefaultProfiles.SaveProfileConfigurations(ProfileConfigurationPath(), profiles);
                return true;
            }
            return false;
        }
    }
}
