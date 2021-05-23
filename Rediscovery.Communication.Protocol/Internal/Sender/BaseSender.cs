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

        public void Send(byte[] data, int port, Action<TransportState> successCallback)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        Socket sender = Network.CreateSocket(port);
                        sender.Connect(Network.LocalEndPoint(port));
                        var raw = new List<byte>(data);
                        raw.AddRange(Network.EOFBytes);
                        var bytes = raw.ToArray();
                        sender.BeginSend(bytes, 0, bytes.Length, 0,
                            new AsyncCallback(OnSendCallback),
                            new StateObjectSender
                            {
                                Sender = sender,
                                SuccessCallback = successCallback
                            });
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        successCallback?.Invoke(TransportState.Error);
                    }
                });
            } catch (Exception ex)
            {
                _logger.Error(ex);
                successCallback?.Invoke(TransportState.Error);
            }
        }

        private void OnSendCallback(IAsyncResult ar)
        {
            StateObjectSender stateObject = null;
            try
            {
                stateObject = (StateObjectSender)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = stateObject.Sender.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                stateObject.SuccessCallback?.Invoke(TransportState.Ok);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                stateObject?.SuccessCallback?.Invoke(TransportState.Error);
            }
        }
    }
}
