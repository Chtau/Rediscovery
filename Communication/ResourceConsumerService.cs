using CommunicationBase;
using Grpc.Core;
using SharedBase.Feature;
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
        public event EventHandler<List<SharedBase.Device.DeviceInfo>> ReceiveActiveDevices;
        public event EventHandler<List<SharedBase.Device.DeviceInfo>> ReceivePendingDevices;
        public event EventHandler<List<SharedBase.Device.DeviceInfo>> ReceiveDevices;
        public event EventHandler<List<SharedBase.Device.FeatureDefinitionExtended>> ReceiveFeatures;
        public event EventHandler<SharedBase.Device.DeviceInfo> ReceiveUpdateDevices;
        public event EventHandler<(Guid deviceId, bool result)> ReceiveDeleteDevicesResult;
        public event EventHandler<(Guid deviceId, bool accept)> ReceiveResolvePendingDevicesResult;
        public event EventHandler<(Guid featureId, string profileId, bool result)> ReceiveFeatureDetailProfileDeleteResult;
        public event EventHandler<(FeatureProfil profile, bool result)> ReceiveFeatureDetailProfileSave;
        public event EventHandler<(FeatureSetting setting, bool result)> ReceiveFeatureDetailSettingSave;
        public event EventHandler<Models.FeatureDetail> ReceiveFeatureDetails;

        private readonly ILogger _logger;

        private Resources.ResourceExchange.ResourceExchangeClient resourceExchangeClient;

        public ResourceConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public bool Connect(string ipAddress, int port, string certificatePEM)
        {
            try
            {
                var channelCredentials = new SslCredentials(certificatePEM);
                Channel channel = new Channel(ipAddress, port, channelCredentials);
                resourceExchangeClient = new Resources.ResourceExchange.ResourceExchangeClient(channel);
                return resourceExchangeClient != null;
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
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
                                var result = new List<SharedBase.Device.DeviceInfo>();
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
                                var result = new List<SharedBase.Device.DeviceInfo>();
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
                                var result = new List<SharedBase.Device.DeviceInfo>();
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

        public void UpdateDevice(string token, SharedBase.Device.DeviceInfo deviceInfo)
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

        public void DeleteDevice(string token, Guid deviceId)
        {
            Task.Run(async () =>
            {
                var cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    _logger.LogTrace("Consumer send delete Device request");
                    var msg = new Resources.DeviceChangeRequest
                    {
                        Id = deviceId.ToString(),
                        Result = Resources.DeviceChangeRequest.Types.ActionResult.None
                    };
                    var reply = await resourceExchangeClient.DeleteDeviceAsync(msg, cancellationToken: cts.Token, headers: meta);
                    (Guid deviceId, bool result) result = (reply.Id.SafeGuid(), reply.Result == Resources.DeviceChangeRequest.Types.ActionResult.Ok ? true : false);
                    ReceiveDeleteDevicesResult?.Invoke(this, result);
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

        public void ResolvePendingDevice(string token, Guid deviceId, bool accept)
        {
            Task.Run(async () =>
            {
                var cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    _logger.LogTrace("Consumer send resolve pending Device request");
                    var msg = new Resources.DevicePendingRequest
                    {
                        Id = deviceId.ToString(),
                        Accept = accept
                    };
                    var reply = await resourceExchangeClient.ResolvePendingDeviceAsync(msg, cancellationToken: cts.Token, headers: meta);
                    (Guid deviceId, bool accept) result = (reply.Id.SafeGuid(), reply.Result == Resources.DeviceChangeRequest.Types.ActionResult.Ok ? true : false);
                    ReceiveResolvePendingDevicesResult?.Invoke(this, result);
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

        public void FeatureDetail(string token, FeatureSetting setting)
        {
            Task.Run(async () =>
            {
                var cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    _logger.LogTrace("Consumer send feature detail request");
                    var msg = new Resources.FeatureDetailRequest
                    {
                        FeatureId = setting.FeatureId.ToString()
                    };
                    var reply = await resourceExchangeClient.FeatureDetailAsync(msg, cancellationToken: cts.Token, headers: meta);
                    var result = new Models.FeatureDetail
                    {
                        FeatureId = reply.FeatureId.SafeGuid(),
                        Setting = reply.Setting.GetDeviceFeatureSetting(),
                        Profils = new List<FeatureProfil>()
                    };
                    if (reply.Profiles.Count > 0)
                    {
                        foreach (var item in reply.Profiles)
                        {
                            result.Profils.Add(item.GetDeviceFeatureProfil());
                        }
                    }
                    ReceiveFeatureDetails?.Invoke(this, result);
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
