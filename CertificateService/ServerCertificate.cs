using CertificateManager;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateService
{
    public static class ServerCertificate
    {
        public static void Create(string outputDirectory, string dnsNameOrIP, string password = "1234", string certFileName = "development", string friendlyName = null)
        {
            var serviceProvider = new ServiceCollection()
                                .AddCertificateManager()
                                .BuildServiceProvider();

            var _createCertificatesRsa = serviceProvider.GetService<CreateCertificatesRsa>();

            var devCertificate = _createCertificatesRsa
                .CreateDevelopmentCertificate(dnsNameOrIP, 10);
            devCertificate.FriendlyName = "development";
            if (!string.IsNullOrWhiteSpace(friendlyName))
                devCertificate.FriendlyName = friendlyName;

            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            // full pfx with password
            var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, devCertificate);
            File.WriteAllBytes(Path.Combine(outputDirectory, $"{certFileName}.pfx"), rootCertInPfxBtyes);

            // private key
            var exportRsaPrivateKeyPem = importExportCertificate.PemExportRsaPrivateKey(devCertificate);
            File.WriteAllText(Path.Combine(outputDirectory, $"{certFileName}.key"), exportRsaPrivateKeyPem);

            // public key certificate as pem
            var exportPublicKeyCertificatePem = importExportCertificate.PemExportPublicKeyCertificate(devCertificate);
            File.WriteAllText(Path.Combine(outputDirectory, $"{certFileName}.pem"), exportPublicKeyCertificatePem);
        }
    }
}
