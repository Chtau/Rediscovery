using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Models
{
    public class PluginVersion
    {
        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public string Label { get; set; }
    }
}
