using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationBase
{
    public class ConsumerConnectionConfiguration
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int SSLPort { get; set; }
        public string CertificatePEM { get; set; }
        public bool UseSSL { get; set; }
    }
}
