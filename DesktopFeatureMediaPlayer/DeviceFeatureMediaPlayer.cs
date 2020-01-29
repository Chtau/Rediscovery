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

        public event EventHandler<DeviceFeatureData> SendData;

        public DeviceFeatureMediaPlayer()
        {
            OnAddDefaultControllers();
        }

        public void Dispose()
        {

        }

        public DeviceFeature GetDeviceFeatureInfo()
        {
            return new DeviceFeature
            {
                DisplayName = "Media Player",
                Id = new Guid("36CCEE18-583F-4ED9-82E9-3033495665DB"),
                ControlIntegrationPoint = DeviceFeature.IntegrationPoint.Mobile,
                FeatureIntegrationPoint = DeviceFeature.IntegrationPoint.Desktop,
                ControlIntegration = DeviceFeature.ControlIntegrationType.None,
                MinControlIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
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
                if (currentDeviceFeatureData.Data is SharedCoreModels.FeatureModels.MediaPlayer.ClientCommandSendModel commandModel && commandModel != null)
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

        private void OnAddDefaultControllers()
        {
            if (controllers == null)
                controllers = new List<MediaPlayerController>();
            var profiles = MediaPlayerDefaultProfiles.GetProfileConfigurations();
            if (profiles != null && profiles.Count > 0)
            {
                foreach (var item in profiles)
                {
                    var controller = new MediaPlayerController(item);
                    controller.InitWatcher();
                    controllers.Add(controller);
                }
            }
        }

        private MediaPlayerController OnGetController(Guid profileId)
        {
            return controllers.FirstOrDefault(x => x.ProfileConfiguration.Id == profileId);
        }
    }
}
