using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public static class ModelExtensions
    {
        public static ConnectionListenConfiguration GetListenConfiguration(this ConnectionConfiguration connectionConfiguration)
        {
            return new ConnectionListenConfiguration
            {
                Port = connectionConfiguration.ListenPort,
                PackageSize = connectionConfiguration.PackageSize
            };
        }
    }
}
