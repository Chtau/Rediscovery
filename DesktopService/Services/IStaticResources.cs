using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DesktopService.Services
{
    public interface IStaticResources
    {
        string HostIpAddress { get; set; }
        ushort HostPort { get; set; }
        ushort HostPortHttps { get; set; }
        string ExePath { get; set; }
        X509Certificate2 X509Certificate2 { get; set; }
        string PEM { get; set; }
        SharedBase.Connection.Manifest ServiceManifest { get; set; }
        bool SSLActive { get; set; }
        string DiscoveryServiceFolderName { get; set; }
        string PluginFolderName { get; set; }
        string PluginHiddenBackupFolderName { get; set; }
    }
}
