using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Models
{
    public class PluginExchangeEntity<T>
    {
        public string Sid { get; set; }
        public T Entity { get; set; }
    }
}
