using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test
{
    public class SendReceiveWithProxy
    {
        [Fact]
        public void SendReceiveSimpleTextWithProxy()
        {
            string content = "Test";
            IRediscoveryProtocol protocolProxy = new RediscoveryProtocol();
            protocolProxy.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    ListenerDeactivated = true,
                    Connection = new Models.ConnectionConfiguration(16571, 16581, 1024)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16596, 16597, 1024),
                    ConnectionLargeData = new Models.ConnectionConfiguration(16498, 16499, 1024 * 60)
                }
            });

            IRediscoveryProtocol protocol2 = new RediscoveryProtocol();
            protocol2.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16581, Models.DiscoveryConfiguration.DefaultSendPortDiscovery, 1024)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16586, 16587, 1024),
                    ConnectionLargeData = new Models.ConnectionConfiguration(16488, 16489, 1024 * 60)
                }
            });

            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.Start(null);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            bool stop = false;
            string data = null;
            Task.Run(async () =>
            {
                protocol2.Listen<string>((transfer) =>
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
                protocol.Send(new Transfer<string>(protocol2.Identifer, content));
            });

            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (!stop);
            Assert.True(data == content, "No Data received via Socket");

            protocol2.Stop();
            protocolProxy.Stop();
            protocol.Stop();
        }
    }
}
