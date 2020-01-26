using System;
using System.Collections.Generic;
using System.Text;
using SharedCoreModels.DeviceFeature;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace DesktopService.Features.DeviceFeature
{
    public class FeatureService : IFeatureService
    {
        private List<IDeviceFeatureImplementation> deviceFeatureImplementations = new List<IDeviceFeatureImplementation>();
        private readonly IHubContext<DeviceFeatureHub> _hubContext;

        public FeatureService(IHubContext<DeviceFeatureHub> hubContext)
        {
            _hubContext = hubContext;
            Load();
        }

        public IDeviceFeatureImplementation GetFeature(Guid featureId)
        {
            return deviceFeatureImplementations.FirstOrDefault(x => x.GetDeviceFeatureInfo().Id == featureId);
        }

        public List<SharedCoreModels.DeviceFeature.DeviceFeature> GetFeaturesManifest()
        {
            var manifest = new List<SharedCoreModels.DeviceFeature.DeviceFeature>();
            foreach (var item in deviceFeatureImplementations)
            {
                manifest.Add(item.GetDeviceFeatureInfo());
            }
            return manifest;
        }

        public void Load()
        {
            var console = new DesktopFeatureConsole.DeviceFeatureConsole();
            console.SendData += (object sender, DeviceFeatureData e) =>
            {
                System.Diagnostics.Debug.Print("Feature response =>" + e.Data);
                ResponseToClient(console.GetDeviceFeatureInfo().Id, e);
            };
            deviceFeatureImplementations.Add(console);
            var mediaPlayer = new DesktopFeatureMediaPlayer.DeviceFeatureMediaPlayer();
            mediaPlayer.SendData += (object sender, DeviceFeatureData e) =>
            {
                System.Diagnostics.Debug.Print("Feature response =>" + e.Data);
                ResponseToClient(console.GetDeviceFeatureInfo().Id, e);
            };
            deviceFeatureImplementations.Add(mediaPlayer);
        }

        private void ResponseToClient(Guid featureId, DeviceFeatureData data)
        {
            _hubContext.Clients.User(data.DeviceId).SendAsync("ClientResponse", featureId, data.Data);
            //Clients.Caller.SendAsync("ClientResponse", featureId, data);
        }
    }
}
