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

            var devCertificate = OnCreateCertificate(serviceProvider, dnsNameOrIP, friendlyName);

            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            // full pfx with password
            File.WriteAllBytes(Path.Combine(outputDirectory, $"{certFileName}.pfx"), OnGetPfx(importExportCertificate, devCertificate, password));

            // private key
            File.WriteAllText(Path.Combine(outputDirectory, $"{certFileName}.key"), OnGetPrivateKey(importExportCertificate, devCertificate));

            // public key certificate as pem
            File.WriteAllText(Path.Combine(outputDirectory, $"{certFileName}.pem"), OnGetPem(importExportCertificate, devCertificate));
        }

        public static byte[] CreatePfx(string dnsNameOrIP, string password = "1234", string friendlyName = null)
        {
            var serviceProvider = new ServiceCollection()
                                .AddCertificateManager()
                                .BuildServiceProvider();

            var devCertificate = OnCreateCertificate(serviceProvider, dnsNameOrIP, friendlyName);

            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            return OnGetPfx(importExportCertificate, devCertificate, password);
        }

        public static X509Certificate2 Create(string dnsNameOrIP, string friendlyName = null)
        {
            var serviceProvider = new ServiceCollection()
                                .AddCertificateManager()
                                .BuildServiceProvider();

            return OnCreateCertificate(serviceProvider, dnsNameOrIP, friendlyName);
        }

        public static string GetPrivateKey(X509Certificate2 certificate)
        {
            var serviceProvider = new ServiceCollection()
                                .AddCertificateManager()
                                .BuildServiceProvider();
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            return OnGetPrivateKey(importExportCertificate, certificate);
        }

        public static string GetPEM(X509Certificate2 certificate)
        {
            var serviceProvider = new ServiceCollection()
                                .AddCertificateManager()
                                .BuildServiceProvider();
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            return OnGetPem(importExportCertificate, certificate);
        }

        private static X509Certificate2 OnCreateCertificate(IServiceProvider serviceProvider, string dnsNameOrIP, string friendlyName)
        {
            var _createCertificatesRsa = serviceProvider.GetService<CreateCertificatesRsa>();

            var devCertificate = _createCertificatesRsa
                .CreateDevelopmentCertificate(dnsNameOrIP, 10);
            devCertificate.FriendlyName = "development";
            if (!string.IsNullOrWhiteSpace(friendlyName))
                devCertificate.FriendlyName = friendlyName;

            return devCertificate;
        }

        private static byte[] OnGetPfx(ImportExportCertificate importExportCertificate, X509Certificate2 certificate, string password)
        {
            return importExportCertificate.ExportRootPfx(password, certificate);
        }

        private static string OnGetPrivateKey(ImportExportCertificate importExportCertificate, X509Certificate2 certificate)
        {
            return importExportCertificate.PemExportRsaPrivateKey(certificate);
        }

        private static string OnGetPem(ImportExportCertificate importExportCertificate, X509Certificate2 certificate)
        {
            return importExportCertificate.PemExportPublicKeyCertificate(certificate);
        }
    }
}
