using Avalonia.Threading;
using IPCPipe.Models;
using PluginFeature.Models;
using SharedCoreModels.DeviceFeature;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DesktopHub.Features
{
    public class FeaturesControlViewModel : BaseViewModel
    {
        private readonly IPCPipe.IPipeResourceProvider _resourceProvider;
        private readonly IPCPipe.IPipeClient _pipeClient;

        public ObservableCollection<DeviceFeatureDefinition> Items { get; set; } = new ObservableCollection<DeviceFeatureDefinition>();

        public FeaturesControlViewModel()
        {
            _resourceProvider = (IPCPipe.IPipeResourceProvider)Program.ServiceProvider.GetService(typeof(IPCPipe.IPipeResourceProvider));
            _pipeClient = (IPCPipe.IPipeClient)Program.ServiceProvider.GetService(typeof(IPCPipe.IPipeClient));
        }

        public void Refresh()
        {
            _resourceProvider.Receiver<List<DeviceFeatureDefinition>>("rediscoveryservice", "features", OnReceiveResource);
        }

        private void OnReceiveResource(PipeResource<List<DeviceFeatureDefinition>> obj)
        {
            Dispatcher.UIThread.Post(() =>
            {
                Items.Clear();
                foreach (var item in obj.Entity)
                {
                    Items.Add(item);
                }
            });
        }
    }
}
