using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public abstract class BaseConfiguration
    {
        public const int DefaultPackageSize = 1024;

        public ConnectionConfiguration Connection { get; set; }
    }
}
