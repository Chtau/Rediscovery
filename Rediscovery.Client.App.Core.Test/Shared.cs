using Rediscovery.Client.Shared.Core.Features.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core
{
    public static class Shared
    {
        public static void Init(int discoveryPort = 14545)
        {
            CoreClientManager.Init(new CoreClientManagerSetting
            {
                CurrentDiscoverSetting = new Features.Discovery.DiscoverSetting
                {
                    Port = discoveryPort
                },
                CurrentStorageSetting = new StorageSetting
                {
                    DatabaseFile = null
                }
            });
        }
    }
}
