using Rediscovery.Client.App.Core.Features.Discovery;
using Rediscovery.Client.Service.Discovery;
using Rediscovery.Client.Shared.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Client.App.Core
{
    public class DiscoverTest
    {
        [Fact]
        public void DiscoverSingleDevice()
        {
            Rediscovery.Shared.Base.Discovery.DiscoveryServiceInfo resultInfo = null;
            string serviceResult = null;
            int discoveryPort = 14545;
            Shared.Init(discoveryPort);
            // init discovery service
            var dc = new DiscoveryClient();
            dc.Start(new Rediscovery.Shared.Base.Discovery.DiscoveryServiceInfo
            {
                DesktopName = "Test",
                DesktopOS = "XUnit",
                IPAddress = "127.0.0.1",
                Port = 1234
            }, discoveryPort, (result) =>
            {
                serviceResult = result;
            });
            var dis = Resolver.Get<IDiscoverDevices>();
            dis.Start((result) =>
            {
                if (result != null)
                {
                    resultInfo = result;
                }
            });
            Task.WaitAny(Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
                } while (resultInfo == null || string.IsNullOrWhiteSpace(serviceResult));
            }), Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
            }));

            Assert.True(resultInfo.DesktopName == "Test", $"Device received {nameof(Rediscovery.Shared.Base.Discovery.DiscoveryServiceInfo)} object");
            Assert.True(!string.IsNullOrWhiteSpace(serviceResult), $"Service received public Client address");
        }
    }
}
