using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.Models
{
    public class CurrentConnection
    {
        public string IP { get; set; } = "";
        public int Port { get; set; } = 0;
        public int PortSSL { get; set; } = 0;
        public string DeviceIdentifier { get; set; } = "";
        public string Token { get; set; } = null;
        public string Pem { get; set; } = null;
        public bool UseSSL { get; set; } = false;

        public Communication.Base.ConsumerConnectionConfiguration ConnectionConfiguration
        {
            get
            {
                return new Communication.Base.ConsumerConnectionConfiguration
                {
                    CertificatePEM = Pem,
                    IPAddress = IP,
                    Port = Port,
                    SSLPort = PortSSL,
                    UseSSL = UseSSL
                };
            }
        }
    }
}
