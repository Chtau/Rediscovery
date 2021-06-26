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
            
            IRediscoveryProtocol protocolReceiver = new RediscoveryProtocol();
            protocolReceiver.SetMetadata("8FB2D46ED8DDA5D9", "Receiver", Models.DeviceMetadata.IdiomType.Undefined);
            protocolReceiver.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    ListenerDeactivated = true,
                    Connection = new Models.ConnectionConfiguration(16571, 16581, 1024)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16596, 16597, 1024),
                },
                Large = new Models.LargeConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16498, 16499, 1024 * 60)
                }
            });
            
            IRediscoveryProtocol protocolProxy = new RediscoveryProtocol();
            protocolProxy.SetMetadata("091812ED97D4F55A", "Proxy", Models.DeviceMetadata.IdiomType.Undefined);
            protocolProxy.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16581, Models.DiscoveryConfiguration.DefaultSendPort, 1024)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16586, 16587, 1024),
                },
                Large = new Models.LargeConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(16488, 16489, 1024 * 60)
                }
            });
            
            IRediscoveryProtocol protocol = new RediscoveryProtocol();
            protocol.SetMetadata("08232A238D844317", "Sender", Models.DeviceMetadata.IdiomType.Undefined);
            protocol.Start(null);
            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();

            bool stop = false;
            string data = null;
            Task.Run(async () =>
            {
                protocolReceiver.Listen<string>((transfer) =>
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
                protocol.Send(new Transfer<string>(protocolReceiver.Identifer, content));
            });

            do
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
            } while (!stop);
            Assert.True(data == content, "No Data received via Socket");

            protocolReceiver.Stop();
            protocolProxy.Stop();
            protocol.Stop();
        }
    }
}
