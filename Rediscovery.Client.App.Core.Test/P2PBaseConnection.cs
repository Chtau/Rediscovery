using Rediscovery.Client.App.Core.Features.Device;
using Rediscovery.Client.Shared.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Client.App.Core
{
    public class P2PBaseConnection
    {
        [Fact]
        public async void Probe()
        {
            int port = 24567;
            var firstConfigId = Guid.NewGuid();
            var secondConfigId = Guid.NewGuid();
            Shared.Init();

            var firstClientTask = Task.Run(() =>
            {
                var dm = Resolver.Get<IDevicesManager>();
                dm.AddOrUpdateConnectionConfiguration(new Features.Device.Models.ConnectionConfiguration
                {
                    Id = firstConfigId,
                    Address = Feature.Shared.Functions.NetworkAddress.GetIpAddr(),
                    Port = port
                });
                Assert.True(dm.Probe(firstConfigId), "Could not reach Service with Probe");
            });
            var secondClientTask = Task.Run(() =>
            {
                var dm = Resolver.Get<IDevicesManager>();
                dm.AddOrUpdateConnectionConfiguration(new Features.Device.Models.ConnectionConfiguration
                {
                    Id = secondConfigId,
                    Address = Feature.Shared.Functions.NetworkAddress.GetIpAddr(),
                    Port = port
                });
                Assert.True(dm.Probe(secondConfigId), "Could not reach Service with Probe");
            });
            
            
            /*var dm = Resolver.Get<IDevicesManager>();
            dm.AddOrUpdateConnectionConfiguration(new Features.Device.Models.ConnectionConfiguration
            {
                Id = configId,
                Address = Feature.Shared.Functions.NetworkAddress.GetIpAddr(),
                Port = port
            });
            Assert.True(dm.Probe(configId), "Could not reach Service with Probe");*/
        }
    }
}
