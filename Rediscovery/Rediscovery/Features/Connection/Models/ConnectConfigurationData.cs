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
        public bool UseSSL { get; set; }
        public int Port { get; set; }
    }
}
