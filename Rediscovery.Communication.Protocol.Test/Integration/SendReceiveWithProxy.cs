using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test.Integration
{
    public class SendReceiveWithProxy
    {
        [Fact]
        public void SendReceiveSimpleTextWithProxy()
        {
            string content = "Test";
            
            IRediscoveryProtocol protocolReceiver = Shared.TestDevice(1000, 2000);
            protocolReceiver.SetMetadata("8FB2D46ED8DDA5D9", "Receiver", Models.DeviceMetadata.IdiomType.Undefined);
            
            IRediscoveryProtocol protocolProxy = Shared.TestDevice(0, 1000);
            protocolProxy.SetMetadata("091812ED97D4F55A", "Proxy", Models.DeviceMetadata.IdiomType.Undefined);

            IRediscoveryProtocol protocol = Shared.TestDevice(0, 0);
            protocol.SetMetadata("08232A238D844317", "Sender", Models.DeviceMetadata.IdiomType.Undefined);
            //protocol.Start(null);
            Task.Delay(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();

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
