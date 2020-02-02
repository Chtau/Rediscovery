using SharedCoreModels.DeviceFeature;
using SharedCoreModels.FeatureModels.MediaPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopFeatureMediaPlayer
{
    public class DeviceFeatureMediaPlayer : IDeviceFeatureImplementation
    {
        private DeviceFeatureData currentDeviceFeatureData;
        private List<MediaPlayerController> controllers = new List<MediaPlayerController>();
        private ProfileConfiguration currentProfileConfiguration;

        public event EventHandler<DeviceFeatureData> SendData;

        public DeviceFeatureMediaPlayer(ProfileConfiguration profileConfiguration)
        {
            currentProfileConfiguration = profileConfiguration;
            var controller = new MediaPlayerController(currentProfileConfiguration);
            controller.InitWatcher();
            controllers.Add(controller);
        }

        public void Dispose()
        {

        }

        public DeviceFeature GetDeviceFeatureInfo()
        {
            return new DeviceFeature
            {
                DisplayName = currentProfileConfiguration.DisplayName,
                Id = currentProfileConfiguration.Id,
                ControlIntegrationPoint = DeviceFeature.IntegrationPoint.Mobile,
                FeatureIntegrationPoint = DeviceFeature.IntegrationPoint.Desktop,
                ControlIntegration = DeviceFeature.ControlIntegrationType.MediaPlayer,
                MinControlIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                SettingsObject = currentProfileConfiguration.CommandAvailable,
                Version = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
            };
        }

        public void Init()
        {
            
        }

        public void ReceiveData(DeviceFeatureData data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.Data?.ToString()))
            {
                currentDeviceFeatureData = data;
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

        private void OnHandleCommand(ClientCommandSendModel commandModel)
        {
            var controller = OnGetController(commandModel.ProfileId);
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
    }
}
