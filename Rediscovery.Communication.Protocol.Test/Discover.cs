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
                    Connection = new Models.ConnectionConfigurationDiscovery(16571, 16574, 1024)
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
                    Connection = new Models.ConnectionConfigurationDiscovery(16574, Models.DiscoveryConfiguration.DefaultSendPort, 1024)
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

        [Fact]
        public async void ListenPeerMultiHops()
        {
            IRediscoveryProtocol protocolPP2 = Shared.TestDevice(11000, 21000);
            IRediscoveryProtocol protocolPP1 = Shared.TestDevice(11000, 20000);

            IRediscoveryProtocol protocolP2 = Shared.TestDevice(1000, 11000);
            IRediscoveryProtocol protocolP1 = Shared.TestDevice(1000, 10000);

            IRediscoveryProtocol protocol3 = Shared.TestDevice(0, 1000);

            IRediscoveryProtocol protocol1 = Shared.TestDevice(0, 200);

            IRediscoveryProtocol protocol2 = Shared.TestDevice(0, 100);

            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.True(protocol.Devices.Count == 7, "Discover device number missmatch");
            Assert.True(protocol.Devices.Count(x => x.Hops == 0) == 3, "Peer connection should be direct");
            Assert.True(protocol.Devices.Count(x => x.Hops == 1) == 2, "Peer connection should be with 1 hop");
            Assert.True(protocol.Devices.Count(x => x.Hops == 2) == 2, "Peer connection should be with 2 hops");

            protocol.Stop();
            protocol1.Stop();
            protocol2.Stop();
            protocol3.Stop();
            protocolP1.Stop();
            protocolP2.Stop();
            protocolPP1.Stop();
            protocolPP2.Stop();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async void ListenPeerMultiHopsRange()
        {
            int portListenOffest = 1000;
            int portSendOffest = 1500;

            IRediscoveryProtocol protocolRange = new RediscoveryProtocol();
            protocolRange.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    Connection = new Models.ConnectionConfigurationDiscovery(
                        new List<int> 
                        { 
                            Models.DiscoveryConfiguration.DefaultListenPort + 101,
                            Models.DiscoveryConfiguration.DefaultListenPort + 102,
                            Models.DiscoveryConfiguration.DefaultListenPort + 103,
                            Models.DiscoveryConfiguration.DefaultListenPort + 104
                        }, new List<int> { Models.DiscoveryConfiguration.DefaultSendPort }, Models.DiscoveryConfiguration.DefaultPackageSize)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(Models.DataConfiguration.DefaultListenPort + portListenOffest, Models.DataConfiguration.DefaultSendPort + portSendOffest, Models.DataConfiguration.DefaultPackageSize)
                },
                Large = new Models.LargeConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(Models.LargeConfiguration.DefaultListenPort + portListenOffest, Models.LargeConfiguration.DefaultSendPort + portSendOffest, Models.LargeConfiguration.DefaultPackageSize * 60)
                },
                Handshake = new Models.HandshakeConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(Models.HandshakeConfiguration.DefaultListenPort + portListenOffest, Models.HandshakeConfiguration.DefaultSendPort + portSendOffest, Models.HandshakeConfiguration.DefaultPackageSize)
                }
            });
            //await Task.Delay(TimeSpan.FromMinutes(5));
            //return;
            IRediscoveryProtocol protocol4 = Shared.TestDevice(104, 400);
            IRediscoveryProtocol protocol3 = Shared.TestDevice(103, 300);
            IRediscoveryProtocol protocol1 = Shared.TestDevice(102, 200);
            IRediscoveryProtocol protocol2 = Shared.TestDevice(101, 100);

            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.True(protocol.Devices.Count == 5, "Discover device number missmatch");
            Assert.True(protocol.Devices.Count(x => x.Hops == 0) == 1, "Peer connection should be direct");
            Assert.True(protocol.Devices.Count(x => x.Hops == 1) == 4, "Peer connection should be with 1 hop");

            protocol.Stop();
            protocol1.Stop();
            protocol2.Stop();
            protocol3.Stop();
            protocol4.Stop();
            protocolRange.Stop();

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
