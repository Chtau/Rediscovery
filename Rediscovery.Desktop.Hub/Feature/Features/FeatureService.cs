using IPCPipe.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Features
{
    public class FeatureService : IFeatureService
    {
        private readonly IPCPipe.IPipeResourceProvider _resourceProvider;
        private readonly IPCPipe.IPipeServer _pipeServer;

        public List<DeviceFeature> Items { get; set; } = new List<DeviceFeature>();

        public event EventHandler<List<DeviceFeature>> DeviceFeatureReceived;

        public FeatureService(IPCPipe.IPipeResourceProvider pipeResourceProvider,
            IPCPipe.IPipeServer pipeServer)
        {
            _resourceProvider = pipeResourceProvider;
            _pipeServer = pipeServer;
        }

        public void Init()
        {
            _resourceProvider.Receiver<List<DeviceFeature>>("rediscoveryservice", "features", OnReceiveResource);
        }

        private void OnReceiveResource(PipeResource<List<DeviceFeature>> resource)
        {
            Items.Clear();
            Items.AddRange(resource.Entity);
            DeviceFeatureReceived?.Invoke(this, resource.Entity);
        }
    }
}
