using IPCPipe.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.Desktop.Hub.Feature.RemoteResource;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Features
{
    public class FeatureService : IFeatureService
    {
        private readonly IDesktopHubRemoteResourceService _desktopHubRemoteResourceService;

        public List<DeviceFeature> Items { get; set; } = new List<DeviceFeature>();

        public event EventHandler<List<DeviceFeature>> DeviceFeatureReceived;

        public FeatureService(IDesktopHubRemoteResourceService desktopHubRemoteResourceService)
        {
            _desktopHubRemoteResourceService = desktopHubRemoteResourceService;
            _desktopHubRemoteResourceService.ServiceFeatureReceived += _desktopHubRemoteResourceService_ServiceFeatureReceived;
        }

        private void _desktopHubRemoteResourceService_ServiceFeatureReceived(object sender, List<DeviceFeature> e)
        {
            Items.Clear();
            Items.AddRange(e);
            DeviceFeatureReceived?.Invoke(this, e);
        }

        public void Init()
        {
            _desktopHubRemoteResourceService.Connect();
        }
    }
}
