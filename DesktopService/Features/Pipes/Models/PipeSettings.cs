using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Pipes.Models
{
    public class PipeSettings
    {
        public string RediscoveryDesktopHubPath { get; set; }

        public string RediscoveryDiscoveryService { get; set; }

        public bool ShowServiceInfoOnStart { get; set; }
    }
}
