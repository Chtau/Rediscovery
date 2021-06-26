using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test.Integration
{
    public class SendReceiveWithCallbackKey
    {
        [Fact]
        public void SendReceiveFirstListener()
        {
            string content = "Test";
            IRediscoveryProtocol protocol1 = Shared.TestDevice(0, 1000);
            IRediscoveryProtocol protocol = Shared.TestDevice(1000, 0);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            bool stop = false;
            string data = null;
            Task.Run(async () =>
            {
                protocol1.Listen<string>("method1", (transfer) =>
                {
                    data = transfer.Content;
                    stop = true;
                });
                protocol1.Listen<string>("method2", (transfer) =>
                {
                    data = "wrong";
                    stop = true;
                });
                await Task.Delay(TimeSpan.FromMinutes(1));
                stop = true;
            });
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            Task.Run(() =>
            {
                protocol.Send("method1", new Transfer<string>(protocol.Devices[0].Identifier, content));
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
        public void SendReceiveSecondListener()
        {
            string content = "Test";
            IRediscoveryProtocol protocol1 = Shared.TestDevice(0, 1000);
            IRediscoveryProtocol protocol = Shared.TestDevice(1000, 0);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            bool stop = false;
            string data = null;
            Task.Run(async () =>
            {
                protocol1.Listen<string>("method1", (transfer) =>
                {
                    data = "wrong";
                    stop = true;
                });
                protocol1.Listen<string>("method2", (transfer) =>
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
                protocol.Send("method2", new Transfer<string>(protocol.Devices[0].Identifier, content));
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
        public void SendReceiveFallbackListener()
        {
            string content = "Test";
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
                protocol1.Listen<string>("method1", (transfer) =>
                {
                    data = "wrong";
                    stop = true;
                });
                protocol1.Listen<string>("method2", (transfer) =>
                {
                    data = "wrong";
                    stop = true;
                });
                await Task.Delay(TimeSpan.FromMinutes(1));
                stop = true;
            });
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            Task.Run(() =>
            {
                protocol.Send("method0", new Transfer<string>(protocol.Devices[0].Identifier, content));
            });

            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (!stop);
            Assert.True(data == content, "No Data received via Socket");

            protocol1.Stop();
            protocol.Stop();
        }
    }
}
