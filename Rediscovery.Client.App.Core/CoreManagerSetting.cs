using Rediscovery.Client.App.Core.Features.Discovery;
using Rediscovery.Client.App.Core.Features.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core
{
    public class CoreManagerSetting
    {
        public StorageSetting CurrentStorageSetting { get; set; }
        public DiscoverSetting CurrentDiscoverSetting { get; set; }
    }
}
