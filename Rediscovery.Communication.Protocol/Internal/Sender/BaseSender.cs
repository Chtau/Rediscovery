using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal abstract class BaseSender : ISender
    {
        private readonly IProtocolLogger _logger;

        internal Setting setting;

        public virtual int BufferSize => setting.SendPackageBytesData;

        public BaseSender(IProtocolLogger protocolLogger = null)
        {
            _logger = protocolLogger ?? new ProtocolLogger();
        }

        public void Initialize(Setting setting)
        {
            this.setting = setting;
        }

        public void Send(byte[] data, int port)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        Socket sender = Network.CreateSocket(port);
                        sender.Connect(Network.LocalEndPoint(port));// 11000));// new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));// Network.LocalEndPoint(port));
                        var raw = new List<byte>(data);
                        raw.AddRange(Network.EOFBytes);
                        var bytes = raw.ToArray();
                        sender.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(OnSendCallback), sender);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                });
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnSendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                //sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
