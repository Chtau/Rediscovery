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
        }

        [Fact]
        public async void ListenSecondDevice()
        {
            IRediscoveryProtocol protocol1 = new RediscoveryProtocol();
            protocol1.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    ListenerDeactivated = true
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16576, 16577, 1024)
                },
                LowData = new Models.LowDataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16578, 16579, 1024)
                }
            });

            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.True(protocol.Devices.Count == 1, "Discover device number missmatch");
        }
    }
}
