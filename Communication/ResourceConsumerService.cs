using CommunicationBase;
using Grpc.Core;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationResourceConsumer
{
    public class ResourceConsumerService : IResourceConsumerService
    {
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> ReceiveActiveDevices;
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> ReceivePendingDevices;
        public event EventHandler<List<SharedCoreModels.DeviceInfo>> ReceiveDevices;
        public event EventHandler<List<SharedBase.Device.FeatureDefinitionExtended>> ReceiveFeatures;
        public event EventHandler<SharedCoreModels.DeviceInfo> ReceiveUpdateDevices;

        private readonly ILogger _logger;

        private Resources.ResourceExchange.ResourceExchangeClient resourceExchangeClient;

        public ResourceConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public void Connect(string ipAddress, int port, string certificatePEM)
        {
            var channelCredentials = new SslCredentials(certificatePEM);
            Channel channel = new Channel(ipAddress, port, channelCredentials);
            resourceExchangeClient = new Resources.ResourceExchange.ResourceExchangeClient(channel);
        }

        public void ListenActiveDevices(string token, CancellationTokenSource cts = null)
        {
            Task.Run(async () =>
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    using (var call = resourceExchangeClient.ActiveDevices(new Google.Protobuf.WellKnownTypes.Empty(), headers: meta, cancellationToken: cts.Token))
                    {
                        var readTask = Task.Run(async () =>
                        {
                            await foreach (var message in call.ResponseStream.ReadAllAsync())
                            {
                                var result = new List<SharedCoreModels.DeviceInfo>();
                                foreach (var item in message.Devices)
                                {
                                    result.Add(item.GetDeviceInfo());
                                }
                                ReceiveActiveDevices?.Invoke(this, result);
                            }
                        });
                        do
                        {
                            await Task.Delay(100);
                        } while (!cts.IsCancellationRequested);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                finally
                {
                    cts.Cancel();
                }
            });
        }

        public void ListenPendingDevices(string token, CancellationTokenSource cts = null)
        {
            Task.Run(async () =>
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    using (var call = resourceExchangeClient.PendingDevices(new Google.Protobuf.WellKnownTypes.Empty(), headers: meta, cancellationToken: cts.Token))
                    {
                        var readTask = Task.Run(async () =>
                        {
                            await foreach (var message in call.ResponseStream.ReadAllAsync())
                            {
                                var result = new List<SharedCoreModels.DeviceInfo>();
                                foreach (var item in message.Devices)
                                {
                                    result.Add(item.GetDeviceInfo());
                                }
                                ReceivePendingDevices?.Invoke(this, result);
                            }
                        });
                        do
                        {
                            await Task.Delay(100);
                        } while (!cts.IsCancellationRequested);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                finally
                {
                    cts.Cancel();
                }
            });
        }

        public void ListenDevices(string token, CancellationTokenSource cts = null)
        {
            Task.Run(async () =>
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    using (var call = resourceExchangeClient.Devices(new Google.Protobuf.WellKnownTypes.Empty(), headers: meta, cancellationToken: cts.Token))
                    {
                        var readTask = Task.Run(async () =>
                        {
                            await foreach (var message in call.ResponseStream.ReadAllAsync())
                            {
                                var result = new List<SharedCoreModels.DeviceInfo>();
                                foreach (var item in message.Devices)
                                {
                                    result.Add(item.GetDeviceInfo());
                                }
                                ReceiveDevices?.Invoke(this, result);
                            }
                        });
                        do
                        {
                            await Task.Delay(100);
                        } while (!cts.IsCancellationRequested);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                finally
                {
                    cts.Cancel();
                }
            });
        }

        public void ListenFeatures(string token, CancellationTokenSource cts = null)
        {
            Task.Run(async () =>
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    using (var call = resourceExchangeClient.Features(new Google.Protobuf.WellKnownTypes.Empty(), headers: meta, cancellationToken: cts.Token))
                    {
                        var readTask = Task.Run(async () =>
                        {
                            await foreach (var message in call.ResponseStream.ReadAllAsync())
                            {
                                var result = new List<SharedBase.Device.FeatureDefinitionExtended>();
                                foreach (var item in message.Features)
                                {
                                    result.Add(item.GetFeatureDefinition());
                                }
                                ReceiveFeatures?.Invoke(this, result);
                            }
                        });
                        do
                        {
                            await Task.Delay(100);
                        } while (!cts.IsCancellationRequested);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                finally
                {
                    cts.Cancel();
                }
            });
        }

        public void UpdateDevice(string token, SharedCoreModels.DeviceInfo deviceInfo)
        {
            Task.Run(async () =>
            {
                var cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    _logger.LogTrace("Consumer send change update Device request");
                    var reply = await resourceExchangeClient.UpdateDeviceAsync(deviceInfo.GetProtoDeviceInfo(), cancellationToken: cts.Token, headers: meta);
                    ReceiveUpdateDevices?.Invoke(this, reply.GetDeviceInfo());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                finally
                {
                    cts.Cancel();
                }
            });
        }



    }
}
