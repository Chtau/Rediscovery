using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Device.Models
{
    public class ConnectionConfiguration
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }

        public override string ToString()
        {
            return $"Id:\"{Id}\" Address:\"{Address}\" Port:\"{Port}\"";
        }
    }
}
