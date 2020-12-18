using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect.Models
{
    public class ConnectionConfiguration
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
    }
}
