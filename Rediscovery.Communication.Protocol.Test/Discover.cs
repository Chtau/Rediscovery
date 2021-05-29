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
        public void Listen()
        {
            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(50));
            } while (protocol.Devices.Count != 1);
            Assert.True(protocol.Devices.Count == 1, "Discover device number missmatch");
        }
    }
}
