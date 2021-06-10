using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Rediscovery.Communication.Protocol.Test
{

    public static class Shared
    {
        public static IRediscoveryProtocol TestDevice(Models.ConnectionConfiguration connection)
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
                    Connection = connection,
                    ConnectionLargeData = new Models.ConnectionConfiguration(16578, 16579, 1024 * 60)
                }
            });
            return protocol1;
        }
    }
}
