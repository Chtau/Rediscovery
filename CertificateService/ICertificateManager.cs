using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateService
{
    public interface ICertificateManager
    {
        X509Certificate2 CreateNew(string dnsIP);
        X509Certificate2 Certificate(string dnsIP);
        string PEM();
    }
}
