using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public interface IConnectionConfiguration
    {
        int PackageSize { get; }
    }
}
