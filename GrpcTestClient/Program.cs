using CommunicationAuthenticationConsumer;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GrpcTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test Grpc!");
            ConsumeAuthentication();
            /*var consumer = new CommunicationFeatureConsumer.FeatureConsume();
            consumer.Connect("localhost", 5001, ExportToPEM(GetX509Certificate2()));
            consumer.HelloReplay += (obj, message) => {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Hello response]" + message);
                Console.ResetColor();
            };
            Console.WriteLine("Send Client message");
            consumer.SayHello("Test Client");


            Console.WriteLine("Init Feature data stream");
            consumer.ReceivedFeatureData += (obj, data) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Feature data received]" + data.Data);
                Console.ResetColor();
            };*/


            Console.ReadKey();
        }

        private static void ConsumeAuthentication()
        {
            Console.WriteLine("Test Grpc consume Authentication!");
            IAuthenticationConsumerService consumerService = new AuthenticationConsumerService(SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            consumerService.ReceivedManifestReply += (obj, args) =>
            {
                Console.WriteLine("[ReceivedManifestReply] Client:" + args.ClientName);
            };
            consumerService.ReceivedWelcomeReply += (obj, args) =>
            {
                Console.WriteLine($"[ReceivedWelcomeReply] Token:{args.Token} State:{args.State}");
                consumerService.RequestManifest();
            };
            consumerService.Connect("localhost", 5001, ExportToPEM(GetX509Certificate2()));
            consumerService.SendWelcome(new SharedCoreModels.WelcomeDeviceMessage
            {
                DeviceIdentifier = "1",
                DeviceName = "Test Client",
                DeviceType = "Console",
                Idiom = "",
                Manufacturer = "",
                Model = "",
                OSVersion = "",
                Platform = ""
            });
        }

        public static string ExportToPEM(X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }

        public static X509Certificate2 GetX509Certificate2()
        {
            var cert = new X509Certificate2(Path.Combine(@"C:\DEV\TMP", "development.pfx"), "1234");
            Console.WriteLine(cert.FriendlyName + " Issuer:" + cert.Issuer + " Thumbprint:" + cert.Thumbprint);
            return cert;
        }
    }
}
