using CertificateManager;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Rediscovery.Service.Certificate
{
    public class CertificateManager : ICertificateManager
    {
        private X509Certificate2 certificate = null;
        private readonly CreateCertificatesRsa _createCertificatesRsa;

        public CertificateManager(CreateCertificatesRsa createCertificatesRsa)
        {
            _createCertificatesRsa = createCertificatesRsa;
        }

        public X509Certificate2 Certificate(string dnsIP)
        {
            if (certificate == null)
            {
                certificate = ServerCertificate.OnCreateCertificate(_createCertificatesRsa, dnsIP, null);
            }
            return certificate;
        }

        public string PEM()
        {
            return certificate?.ToPEM();
        }

        public X509Certificate2 CreateNew(string dnsIP)
        {
            certificate = null;
            return Certificate(dnsIP);
        }
    }
}
