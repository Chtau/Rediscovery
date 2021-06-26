using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test
{
    public class Discover
    {
        [Fact]
        public async void Listen()
        {
            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.True(protocol.Devices.Count == 0, "Discover device number missmatch");

            protocol.Stop();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async void ListenSecondDevice()
        {
            IRediscoveryProtocol protocol1 = Shared.TestDevice(new Models.ConnectionConfiguration(16576, 16577, 1024));

            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.True(protocol.Devices.Count == 1, "[Protocol] => Discover device number missmatch");
            Assert.True(protocol.Devices[0].Hops == 0, "[Protocol] => Peer connection should be direct");

            protocol.Stop();
            protocol1.Stop();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async void ListenMupltipleDevices()
        {
            IRediscoveryProtocol protocol1 = Shared.TestDevice(new Models.ConnectionConfiguration(16576, 16577, 1024));

            IRediscoveryProtocol protocol2 = Shared.TestDevice(new Models.ConnectionConfiguration(16576, 16577, 1024));

            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.True(protocol.Devices.Count == 2, "Discover device number missmatch");
            Assert.True(protocol.Devices.Count(x => x.Hops == 0) == 2, "Peer connection should be direct");

            protocol.Stop();
            protocol1.Stop();
            protocol2.Stop();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async void ListenPeerHop()
        {
            IRediscoveryProtocol protocol1 = new RediscoveryProtocol();
            protocol1.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    ListenerDeactivated = true,
                    Connection = new Models.ConnectionConfiguration(16571, 16574, 1024)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16476, 16477, 1024)
                },
                Large = new Models.LargeConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16478, 16479, 1024 * 60)
                }
            });

            IRediscoveryProtocol protocol2 = new RediscoveryProtocol();
            protocol2.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16574, Models.DiscoveryConfiguration.DefaultSendPort, 1024)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16576, 16577, 1024)
                },
                Large = new Models.LargeConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16478, 16479, 1024 * 60)
                }
            });

            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.True(protocol.Devices.Count == 2, "Discover device number missmatch");
            Assert.True(protocol.Devices.Count(x => x.Hops != 0) == 1, "Peer device with one hop should exist");

            protocol.Stop();
            protocol1.Stop();
            protocol2.Stop();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
