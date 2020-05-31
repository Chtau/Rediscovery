using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CommunicationBase
{
    public class ConnectionConfiguration
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Token { get; set; }
        public ConnectionState State { get; set; }
        public X509Certificate x509Certificate { get; set; }
    }
}
