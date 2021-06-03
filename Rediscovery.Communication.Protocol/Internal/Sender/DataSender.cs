using Rediscovery.Communication.Protocol.Internal.Listener;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class DataSender : ISender
    {
        private readonly IProtocolLogger _logger;
        private readonly IPackagePipeline _packagePipeline;
        private DataConfiguration configuration;

        public DataSender(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline)
        {
            _logger = protocolLogger;
            _packagePipeline = packagePipeline;
        }

        public void Initialize(BaseConfiguration configuration)
        {
            this.configuration = (DataConfiguration)configuration;
        }

        public void Send<T>(T content, DeviceGreetingReceived deviceGreeting, Action<TransportState> successCallback)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        var endpoint = new IPEndPoint(IPAddress.Parse(deviceGreeting.IP), deviceGreeting.Device.Communication.Data.Port);
                        Socket sender = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        sender.Connect(endpoint);
                        var rawContent = _packagePipeline.Outgoing(content);
                        int sendLength = rawContent.Length + Network.EOFBytes.Length;
                        var raw = new List<byte>(sendLength);
                        raw.AddRange(rawContent);
                        raw.AddRange(Network.EOFBytes);
                        _logger.Trace($"{nameof(DataSender)} send raw data. Peer:{deviceGreeting.Device.Identifier} Hops:{deviceGreeting.Device.Hops} Bytes:{raw.Count}");
                        sender.BeginSend(raw.ToArray(), 0, sendLength, 0,
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

        public void Stop()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);
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
