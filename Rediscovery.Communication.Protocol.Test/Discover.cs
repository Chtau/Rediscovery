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
    }
}
