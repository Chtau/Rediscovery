using IPCPipe.Models;
using Microsoft.AspNetCore.SignalR.Client;
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
        public List<DeviceFeature> Items { get; set; } = new List<DeviceFeature>();

        public event EventHandler<List<DeviceFeature>> DeviceFeatureReceived;

        public FeatureService()
        {
            
        }

        public void Init()
        {
            
        }
    }
}
