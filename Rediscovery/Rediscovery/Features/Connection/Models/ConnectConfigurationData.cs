using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Connection.Models
{
    public class ConnectConfigurationData
    {
        public string Token { get; set; }
        public string PEM { get; set; }
        public int SSLPort { get; set; }
    }
}
