using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Client.App.Core.Features.Discovery;
using Rediscovery.Client.Shared.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core
{
    public class CoreClientManagerSetting : CoreManagerSetting
    {
        public ConnectSetting CurrentConnectSetting { get; set; }
        public DiscoverSetting CurrentDiscoverSetting { get; set; }
    }
}
