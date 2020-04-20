using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Features
{
    public interface IFeatureService
    {
        event EventHandler<List<DeviceFeature>> DeviceFeatureReceived;
        List<DeviceFeature> Items { get; set; }
        void Init();
    }
}
