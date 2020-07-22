using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.Service.Models
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

        public CommunicationBase.ConsumerConnectionConfiguration ConnectionConfiguration
        {
            get
            {
                return new CommunicationBase.ConsumerConnectionConfiguration
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
