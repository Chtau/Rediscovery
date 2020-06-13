using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Models
{
    [Obsolete("Should only be used in Plugin & Desktop Service")]
    public class PluginExchangeEntity<T>
    {
        public string Sid { get; set; }
        public T Entity { get; set; }
    }
}
