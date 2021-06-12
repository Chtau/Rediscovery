using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test
{
    public class Metadata
    {
        [Fact]
        public void SetLongIdentifier()
        {
            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.SetMetadata("ABC1ABC2ABC3ABC4ABC5ABC6", "Device1", Models.DeviceMetadata.IdiomType.Undefined);
            Assert.True(protocol.Identifer == "ABC1ABC2ABC3ABC4", "Identifer length is invalid and not exact 16 Characters");
        }

        [Fact]
        public void SetShortIdentifier()
        {
            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.SetMetadata("ABC1ABC2ABC3", "Device1", Models.DeviceMetadata.IdiomType.Undefined);
            Assert.True(protocol.Identifer == "ABC1ABC2ABC30000", "Identifer length is invalid and not exact 16 Characters");
        }
    }
}
