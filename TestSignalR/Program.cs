using CertificateManager;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TestSignalR
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
.AddCertificateManager()
.BuildServiceProvider();

            //CreateCert(serviceProvider);


            Console.WriteLine("Hello World!");

            var clientCertificate = new X509Certificate2(Path.Combine(@"C:\DEV\Code\Workspaces\Repos\Rediscovery\TestSignalR", "dev_localhost.pfx"), "1234");

            var connectionConfiguration = new CommunicationBase.ConnectionConfiguration
            {
                Address = "192.168.1.100:44342",
                DisplayName = "hub",
                Id = Guid.NewGuid(),
                State = CommunicationBase.ConnectionState.None,
                Token = null,
                SSLThumbprint = clientCertificate.Thumbprint
            };
            var _hub = new CommunicationResourceConsumer.Hub();
            _hub.Init(SharedBase.Logging.DiagnosticsLoggerProvider.Instance, "/remote/resource/hub", CommunicationBase.Protocol.HTTPS);
            _hub.ConnectionStateChanged += _hub_ConnectionStateChanged;

            InitServiceConnection(_hub, connectionConfiguration);
            Console.ReadKey();
        }

        private static void CreateCert(ServiceProvider serviceProvider)
        {

            var _createCertificatesRsa = serviceProvider.GetService<CreateCertificatesRsa>();

            // Create development certificate for localhost
            var devCertificate = _createCertificatesRsa
                .CreateDevelopmentCertificate("localhost", 10);

            devCertificate.FriendlyName = "localhost development";

            string password = "1234";
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            // full pfx with password
            var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, devCertificate);
            File.WriteAllBytes("dev_localhost.pfx", rootCertInPfxBtyes);

            // private key
            var exportRsaPrivateKeyPem = importExportCertificate.PemExportRsaPrivateKey(devCertificate);
            File.WriteAllText($"dev_localhost.key", exportRsaPrivateKeyPem);

            // public key certificate as pem
            var exportPublicKeyCertificatePem = importExportCertificate.PemExportPublicKeyCertificate(devCertificate);
            File.WriteAllText($"dev_localhost.pem", exportPublicKeyCertificatePem);
        }

        private static void _hub_ConnectionStateChanged(object sender, bool e)
        {
            Console.WriteLine("SignalR connection changed:" + e);
        }

        private static async Task<bool> InitServiceConnection(CommunicationResourceConsumer.IHub _hub, CommunicationBase.ConnectionConfiguration connectionConfiguration)
        {
            try
            {
                await _hub.Disconnect();
                _hub.Authenticate(connectionConfiguration.DisplayName, connectionConfiguration, (resultModel, state) =>
                {
                    if (state)
                    {
                        connectionConfiguration.Token = resultModel.Token;
                        _hub.Connect(connectionConfiguration.DisplayName, connectionConfiguration, (listener) =>
                        {
                            if (listener)
                            {
                                _hub.RequestAllData();
                            }
                            else
                            {
                                string msg = "Listener response not valid";
                            }
                        });
                    }
                    else
                    {
                        string msg = "Could not Authenticate for remote resource access";
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return false;
            }
        }
    }
}
