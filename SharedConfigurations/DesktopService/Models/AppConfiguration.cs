using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Configurations.Service.Models
{
    public class AppConfiguration
    {
        public const string SectionName = "AppSettings";

        public string AppDataFolder { get; set; }

        public string FirewallRuleName { get; set; }

        public string[] Plugins { get; set; }

        public string DesktopName { get; set; }

        public string ServiceVersion { get; set; }

        public string AppMinimumVersion { get; set; }

        public string HostIpAddress { get; set; }

        public ushort? HostPort { get; set; }

        public ushort? HostPortHttps { get; set; }
        public bool? HostSSLActive { get; set; }

        public string ServerCertificatePassword { get; set; }

        public string ServerCertificateFriendlyName { get; set; }

        public string DatabasePath { get; set; }

        public string RemoteLogger { get; set; }
    }
}
