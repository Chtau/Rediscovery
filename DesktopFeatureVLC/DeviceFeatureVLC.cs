using SharedCoreModels.DeviceFeature;
using System;

namespace DesktopFeatureVLC
{
    public class DeviceFeatureVLC : IDeviceFeatureImplementation
    {
        private DeviceFeatureData currentDeviceFeatureData;
        private VLC vLC;

        public event EventHandler<DeviceFeatureData> SendData;

        public DeviceFeatureVLC()
        {
            vLC = new VLC();
        }

        public void Dispose()
        {
            
        }

        public DeviceFeature GetDeviceFeatureInfo()
        {
            return new DeviceFeature
            {
                DisplayName = "VLC",
                Id = new Guid("5A3E794B-4CE9-47AB-B7E6-D96FF428CC68"),
                ControlIntegrationPoint = DeviceFeature.IntegrationPoint.Mobile,
                FeatureIntegrationPoint = DeviceFeature.IntegrationPoint.Desktop,
                MinControlIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                MinFeatureIntegrationPoint = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
                Version = new SharedCoreModels.Version() { Major = 0, Minor = 0 },
            };
        }

        public void Init()
        {
            //vLC.VolumneUp();
            //vLC.VolumneUp();
            //vLC.VolumneUp();
            //vLC.PlayPause();
        }

        public void ReceiveData(DeviceFeatureData data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.Data?.ToString()))
            {
                currentDeviceFeatureData = data;
                if (currentDeviceFeatureData.Data is SharedCoreModels.FeatureModels.VLC.VLCCommandModel commandModel && commandModel != null)
                {
                    OnHandleCommand(commandModel);
                } else
                {
                    System.Diagnostics.Debug.Fail("VLC: Unknown object from Data received");
                }
            }
        }

        private void OnHandleCommand(SharedCoreModels.FeatureModels.VLC.VLCCommandModel model)
        {
            switch (model.Command)
            {
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.PlayPause:
                    vLC.PlayPause();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.FullscreenExit:
                    vLC.FullscreenExit();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.Fullscreen:
                    vLC.Fullscreen();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.Next:
                    vLC.Next();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.Previous:
                    vLC.Previous();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.Stop:
                    vLC.Stop();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.Mute:
                    vLC.Mute();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.VolumneUp:
                    vLC.VolumneUp();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.VolumneDown:
                    vLC.VolumneDown();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.SpeedSlower:
                    vLC.SpeedSlower();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.SpeedFaster:
                    vLC.SpeedFaster();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.JumpForward:
                    vLC.JumpForward();
                    break;
                case SharedCoreModels.FeatureModels.VLC.VLCCommandModel.CommandTypes.JumpBackward:
                    vLC.JumpBackward();
                    break;
            }
        }
    }
}
