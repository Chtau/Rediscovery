using System;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test.Integration
{
    public class SendReceive
    {
        [Fact]
        public void SendReceiveSocketSimpleText()
        {
            string content = "Test";
            IRediscoveryProtocol protocol1 = Shared.TestDevice(0, 1000);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            //IRediscoveryProtocol protocol = new RediscoveryProtocol();
            //protocol.Start(null);
            IRediscoveryProtocol protocol = Shared.TestDevice(1000, 0);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            bool stop = false;
            string data = null;
            Task.Run(async () =>
            {
                protocol1.Listen<string>((transfer) =>
                {
                    data = transfer.Content;
                    stop = true;
                });
                await Task.Delay(TimeSpan.FromMinutes(1));
                stop = true;
            });
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
            //System.Threading.Thread.Sleep(TimeSpan.FromMinutes(5));
            Task.Run(() =>
            {
                protocol.Send(new Transfer<string>(protocol.Devices[0].Identifier, content));
            });
            
            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (!stop);
            Assert.True(data == content, "No Data received via Socket");

            protocol1.Stop();
            protocol.Stop();
        }

        [Fact]
        public void SendReceiveLongText()
        {
            string content = string.Join("", System.Linq.Enumerable.Repeat($"{DateTime.UtcNow.Ticks}{DateTime.UtcNow:yyyy-MM-dd-HH-mm-FFFFFFF}", 1000000));
            IRediscoveryProtocol protocol1 = Shared.TestDevice(0, 1000);
            IRediscoveryProtocol protocol = Shared.TestDevice(1000, 0);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            bool stop = false;
            string data = null;
            Task.Run(async () =>
            {
                protocol1.Listen<string>((transfer) =>
                {
                    data = transfer.Content;
                    stop = true;
                });
                await Task.Delay(TimeSpan.FromMinutes(1));
                stop = true;
            });
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            Task.Run(() =>
            {
                protocol.Send(new Transfer<string>(protocol.Devices[0].Identifier, content));
            });

            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (!stop);
            Assert.True(data == content, "No Data received via Socket");

            protocol1.Stop();
            protocol.Stop();
        }

        [Theory]
        [InlineData("BigSample.pdf")]
        public void SendReceiveBigSamplePDF(string fileName)
        {
            string path = System.IO.Path.Combine("..", "..", "..", "SampleFiles", fileName);
            byte[] content = System.IO.File.ReadAllBytes(path);
            IRediscoveryProtocol protocol1 = Shared.TestDevice(0, 1000);
            IRediscoveryProtocol protocol = Shared.TestDevice(1000, 0);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            bool stop = false;
            byte[] data = null;
            Task.Run(async () =>
            {
                protocol1.Listen<byte[]>((transfer) =>
                {
                    data = transfer.Content;
                    stop = true;
                });
                await Task.Delay(TimeSpan.FromMinutes(10));
                stop = true;
            });
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            Task.Run(() =>
            {
                protocol.Send(new Transfer<byte[]>(protocol.Devices[0].Identifier, content));
            });

            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (!stop);
            Assert.True(data.Length == content.Length, "Wrong Data length received");

            protocol1.Stop();
            protocol.Stop();
        }
    }
}
