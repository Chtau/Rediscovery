using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test
{
    public class Diagnostic
    {
        [Fact]
        public void Traffic()
        {
            string content = string.Join("", Enumerable.Repeat($"{DateTime.UtcNow.Ticks}{DateTime.UtcNow:yyyy-MM-dd-HH-mm-FFFFFFF}", 1000000));
            IRediscoveryProtocol protocol1 = Shared.TestDevice(new Models.ConnectionConfiguration(16576, 16577, 1024));
            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
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
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.True(protocol1.Traffic.IncomingPackagesCompleted == 1, "Incoming should have received 1 complete package");
            Assert.True(protocol1.Traffic.IncomingPackageParts == 173, "Incoming should have received 173 package parts");
            Assert.True(protocol.Traffic.OutgoingPackageParts == 173, "Outgoing should have send 173 package parts");

            protocol1.Stop();
            protocol.Stop();
        }

        [Fact]
        public void Timing()
        {
            string content = string.Join("", Enumerable.Repeat($"{DateTime.UtcNow.Ticks}{DateTime.UtcNow:yyyy-MM-dd-HH-mm-FFFFFFF}", 1000000));
            IRediscoveryProtocol protocol1 = Shared.TestDevice(new Models.ConnectionConfiguration(16576, 16577, 1024));
            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
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
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.True(protocol1.Timings.Count == 1, "Incoming should have received timings for 1 device");
            Assert.True(protocol1.Timings[0].Times.Count == 173, "Incoming should have received 173 timings entries for 1 device");
            Assert.True(protocol1.Timings[0].Times.Max() < TimeSpan.FromSeconds(1), "Incoming timings should be below 1 second");

            protocol1.Stop();
            protocol.Stop();
        }
    }
}
