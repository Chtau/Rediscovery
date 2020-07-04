using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DesktopService.Services
{
    public class StaticResources : IStaticResources
    {
        public string HostIpAddress { get; set; }
        public ushort HostPort { get; set; }
        public ushort HostPortHttps { get; set; }
        public string ExePath { get; set; }
        public X509Certificate2 X509Certificate2 { get; set; }
        public string PEM { get; set; }
        public SharedBase.Connection.Manifest ServiceManifest { get; set; }
    }
}
