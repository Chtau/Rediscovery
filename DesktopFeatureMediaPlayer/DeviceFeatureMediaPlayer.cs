using PluginFeature;
using PluginFeature.Models;
using SharedCoreModels.DeviceFeature;
using SharedCoreModels.FeatureModels.MediaPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopFeatureMediaPlayer
{
    public class DeviceFeatureMediaPlayer : BaseDeviceFeature
    {
        private List<MediaPlayerController> controllers = new List<MediaPlayerController>();
        //private ProfileConfiguration currentProfileConfiguration;
        private DateTime updateTimer = DateTime.Now;

        public DeviceFeatureMediaPlayer()//ProfileConfiguration profileConfiguration
        {
            //currentProfileConfiguration = profileConfiguration;
            foreach (var profile in MediaPlayerDefaultProfiles.GetProfileConfigurations())
            {
                var controller = new MediaPlayerController(profile);
                controller.UpdateProcess += Controller_UpdateProcess;
                controllers.Add(controller);
            }
        }

        private void Controller_UpdateProcess(object sender, EventArgs e)
        {
            var newDate = DateTime.Now;
            if ((newDate - updateTimer).TotalSeconds >= 1)
            {
                updateTimer = newDate;
                foreach (var profile in MediaPlayerDefaultProfiles.GetProfileConfigurations())
                {
                    var controller = OnGetController(profile.Id);
                    foreach (var deviceId in RegisteredDevices)
                    {
                        var data = new DeviceFeatureData
                        {
                            Data = new MediaPlayerStateData
                            {
                                ProcessRunning = controller.ProcessRunning,
                                CurrentTitle = controller.CurrentTitle,
                                ProfileId = profile.Id.ToString()
                            },
                            DeviceId = deviceId
                        };
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
            var profiles = new List<DeviceFeatureProfil>();
            var pro = MediaPlayerDefaultProfiles.GetProfileConfigurations();
            if (pro?.Count > 0)
            {
                foreach (var item in pro)
                {
                    profiles.Add(new DeviceFeatureProfil(item.Id.ToString(), item.DisplayName, item.CommandAvailable));
                }
            }
            return new DeviceFeatureDefinition
            {
                DisplayName = "Mediaplayer",
                Id = new Guid("D5B218BC-8F36-4100-9262-71155265DAD7"),
                ControlIntegrationPoint = IntegrationPoint.Mobile,
                FeatureIntegrationPoint = IntegrationPoint.Desktop,
                ControlIntegration = ControlIntegrationType.MediaPlayer,
                MinControlIntegrationPoint = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                SettingsObject = null,
                Version = new PluginFeature.Models.Version() { Major = 0, Minor = 0 },
                Profiles = Newtonsoft.Json.JsonConvert.SerializeObject(profiles)
            };
        }

        public override void Init()
        {
            base.Init();
        }

        public override void ReceiveData(DeviceFeatureData data)
        {
            base.ReceiveData(data);
            if (data != null && IsRegister(data.DeviceId))
            {
                if (!string.IsNullOrWhiteSpace(data.Data?.ToString()))
                {
                    var commandModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCommandSendModel>(data.Data?.ToString());
                    if (commandModel != null)
                    {
                        OnHandleCommand(commandModel);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Fail("MediaPlayer: Unknown object from Data received");
                    }
                }
            }
        }

        private void OnHandleCommand(ClientCommandSendModel commandModel)
        {
            var controller = OnGetController(new Guid(commandModel.ProfileId));
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
                                System.Diagnostics.Debug.Fail($"MediaPlayer: Could not start process (Id:{commandModel.ProfileId})");
                                return;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.Fail($"MediaPlayer: Can't start Process. ApplicationPath is invalid (Id:{commandModel.ProfileId})");
                            return;
                        }
                    }
                    controller.ExecuteCommand(commandModel.Command);
                }
                else
                {
                    System.Diagnostics.Debug.Fail($"MediaPlayer: Process not running and no ApplicationPath to start it (Id:{commandModel.ProfileId})");
                }
            } else
            {
                System.Diagnostics.Debug.Fail($"MediaPlayer: No Controller for Profile (Id:{commandModel.ProfileId})");
            }
        }

        public static List<ProfileConfiguration> GetProfiles()
        {
            return MediaPlayerDefaultProfiles.GetProfileConfigurations();
        }

        private MediaPlayerController OnGetController(Guid profileId)
        {
            return controllers.FirstOrDefault(x => x.ProfileConfiguration.Id == profileId);
        }

        public override void Register(string deviceId)
        {
            base.Register(deviceId);
            foreach (var profile in MediaPlayerDefaultProfiles.GetProfileConfigurations())
            {
                var controller = OnGetController(profile.Id);
                controller.InitWatcher();
            }
        }

        public override void Unregister(string deviceId)
        {
            base.Unregister(deviceId);
            foreach (var profile in MediaPlayerDefaultProfiles.GetProfileConfigurations())
            {
                var controller = OnGetController(profile.Id);
                controller.Stop();
            }
        }
    }
}
