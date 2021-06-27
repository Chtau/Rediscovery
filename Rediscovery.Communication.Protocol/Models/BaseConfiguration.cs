using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public abstract class BaseConfiguration<TConnectionConfiguration> where TConnectionConfiguration : IConnectionConfiguration
    {
        public const int DefaultPackageSize = 1024;

        public TConnectionConfiguration Connection { get; set; }
    }
}
