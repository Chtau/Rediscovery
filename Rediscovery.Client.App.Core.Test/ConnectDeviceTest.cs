using Rediscovery.Client.App.Core.Features.Device;
using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Client.App.Core.Features.Discovery;
using Rediscovery.Client.Service.Discovery;
using Rediscovery.Client.Shared.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Client.App.Core
{
    public class ConnectDeviceTest
    {
        [Fact]
        public void Discover()
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
            
            Assert.True(resultInfo.DesktopName == "Test", $"Client received {nameof(Rediscovery.Shared.Base.Discovery.DiscoveryServiceInfo)} object");
            Assert.True(!string.IsNullOrWhiteSpace(serviceResult), $"Service received public Client address");
        }

        [Fact]
        public async void Probe()
        {
            int port = 24567;
            // first start service
            var serviceTask = Task.Run(() =>
            {
                string portArg = Rediscovery.Shared.Arguments.Service.Arguments.CommandPort + port;
                Rediscovery.Client.App.Service.Program.Main(new string[] { portArg });
            });
            // delay for service startup
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            var configId = Guid.NewGuid();
            Shared.Init();
            var dm = Resolver.Get<IDevicesManager>();
            dm.AddOrUpdateConnectionConfiguration(new Features.Device.Models.ConnectionConfiguration
            {
                Id = configId,
                Address = "192.168.1.101",
                Port = port
            });
            Assert.True(dm.Probe(configId), "Could not reach Service with Probe");
        }

        private static Guid connectionConfigurationId = Guid.NewGuid();

        [Fact]
        public async Task Connect()
        {
            int port = 24568;
            // first start service
            var serviceTask = Task.Run(() =>
            {
                string portArg = Rediscovery.Shared.Arguments.Service.Arguments.CommandPort + port;
                Rediscovery.Client.App.Service.Program.Main(new string[] { portArg });
            });
            // delay for service startup
            await Task.Delay(TimeSpan.FromSeconds(5));

            Shared.Init();
            var dm = Resolver.Get<IDevicesManager>();
            dm.AddOrUpdateConnectionConfiguration(new Features.Device.Models.ConnectionConfiguration
            {
                Id = connectionConfigurationId,
                Address = "192.168.1.101",
                Port = port
            });

            DeviceConnectionState connectionState = null;

            dm.ConnectionStateChanged += (obj, args) =>
            {
                connectionState = args;
            };
            dm.Connect(connectionConfigurationId);
            Task.WaitAny(Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
                } while (string.IsNullOrWhiteSpace(connectionState?.Token));
            }), Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
            }));
            Assert.True(!string.IsNullOrWhiteSpace(connectionState?.Token), "No Token received after try to connect to Service");
        }

        [Fact]
        public async Task Disconnect()
        {
            await Connect();
            var dm = Resolver.Get<IDevicesManager>();
            Assert.True(dm.Disconnect(connectionConfigurationId), "Failed to disconnect from Service");
        }
    }
}
