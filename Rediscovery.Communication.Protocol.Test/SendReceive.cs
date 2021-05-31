using System;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test
{
    public class SendReceive
    {
        [Fact]
        public void SendReceiveSocketSimpleText()
        {
            IRediscoveryProtocol protocol1 = Shared.TestDevice(new Models.ConnectionConfiguration(16576, 16577, 1024));

            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            bool stop = false;
            string data = null;
            Task.Run(async () =>
            {
                protocol.Listen<string>((transfer) =>
                {
                    data = transfer.Content;
                    stop = true;
                });
                await Task.Delay(TimeSpan.FromSeconds(5));
                stop = true;
            });
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            Task.Run(() =>
            {
                protocol.Send(new Transfer<string>(protocol.Devices[0].Identifier, "Test"), (success) =>
                {
                    if (success != TransportState.Ok)
                        throw new Exception("Failed to send");
                });
            });
            
            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (!stop);
            Assert.True(data == "Test", "No Data received via Socket");

            protocol1.Stop();
            protocol.Stop();
        }
    }
}
