using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.InternalIPCModels
{
    public class PendingAuthenticationResolve
    {
        public Guid Id { get; set; }
        public bool Accept { get; set; }
    }
}
