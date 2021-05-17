using System;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test
{
    public class SendReceive
    {
        [Fact]
        public void SendReceiveSocket()
        {
            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            bool stop = false;
            string data = null;
            Task.Run(async () =>
            {
                protocol.Listen((transfer) =>
                {
                    data = System.Text.ASCIIEncoding.ASCII.GetString(transfer.Content);
                    stop = true;
                });
                await Task.Delay(TimeSpan.FromSeconds(5));
                stop = true;
            });
            protocol.Send(null);
            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (!stop);
            Assert.True(!string.IsNullOrWhiteSpace(data), "No Data received via Socket");
        }
    }
}
