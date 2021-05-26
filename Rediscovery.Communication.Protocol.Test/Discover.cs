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
            protocol.Start(new Setting());
            do
            {
                //protocol.
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            } while (true);
        }
    }
}
