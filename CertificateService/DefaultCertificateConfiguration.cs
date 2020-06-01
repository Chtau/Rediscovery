using System;
using System.Collections.Generic;
using System.Text;

namespace CertificateService
{
    internal static class ConfigurationInstance
    {
        internal static DefaultCertificateConfiguration Configuration;
    }

    public class DefaultCertificateConfiguration
    {
        public string DnsIp { get; set; }
    }
}
