using Rediscovery.Client.Shared.Core.Features.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core
{
    public static class Shared
    {
        public static void Init()
        {
            CoreClientManager.Init(new CoreClientManagerSetting
            {
                CurrentDiscoverSetting = new Features.Discovery.DiscoverSetting
                {
                    Port = 14545
                },
                CurrentStorageSetting = new StorageSetting
                {
                    DatabaseFile = null
                }
            });
        }
    }
}
