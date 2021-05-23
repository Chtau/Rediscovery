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
            protocol.Start(new Setting());
            
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
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            Task.Run(() =>
            {
                protocol.Send(new Transfer
                {
                    Content = System.Text.Encoding.ASCII.GetBytes("Test")
                }, (success) =>
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
        }

        [Fact]
        public void SendToIP()
        {
            //192.168.1.102:11000
            RediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Send("192.168.1.102");
        }
    }
}
