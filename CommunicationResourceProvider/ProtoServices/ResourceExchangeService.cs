using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Resources;

namespace CommunicationResourceProvider.ProtoServices
{
    public class ResourceExchangeService : ResourceExchange.ResourceExchangeBase
    {
        private readonly ILogger<ResourceExchangeService> _logger;
        private readonly IResourcesRepository _resourcesRepository;

        private Dictionary<string, IServerStreamWriter<DeviceInfoList>> responseStreamsActiveDevices = new Dictionary<string, IServerStreamWriter<DeviceInfoList>>();
        private Dictionary<string, IServerStreamWriter<DeviceInfoList>> responseStreamsDevices = new Dictionary<string, IServerStreamWriter<DeviceInfoList>>();
        private Dictionary<string, IServerStreamWriter<FeatureList>> responseStreamsFeatures = new Dictionary<string, IServerStreamWriter<FeatureList>>();
        private Dictionary<string, IServerStreamWriter<DeviceInfoList>> responseStreamsPendingDevices = new Dictionary<string, IServerStreamWriter<DeviceInfoList>>();

        public ResourceExchangeService(ILoggerFactory loggerFactory, IResourcesRepository resourcesRepository)
        {
            _logger = loggerFactory.CreateLogger<ResourceExchangeService>();
            _resourcesRepository = resourcesRepository;
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override async Task ActiveDevices(Empty request, IServerStreamWriter<DeviceInfoList> responseStream, ServerCallContext context)
        {
            string sid = null;
            try
            {
                var user = context.GetHttpContext().User;
                sid = user.Claims.GetSid();
                if (responseStreamsActiveDevices.ContainsKey(sid))
                    responseStreamsActiveDevices[sid] = responseStream;
                else
                    responseStreamsActiveDevices.Add(sid, responseStream);

                OnSendActiveDevices(sid);
                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);

                await Task.FromResult(true);
            }
            catch (System.OperationCanceledException)
            {
                _logger.LogTrace("ActiveDevices connection was canceled from Context Cancellation Token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActiveDevices");
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    if (responseStreamsActiveDevices.ContainsKey(sid))
                        responseStreamsActiveDevices.Remove(sid);
                }
            }
        }

        private void OnSendActiveDevices(string sid)
        {
            if (responseStreamsActiveDevices.ContainsKey(sid))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var devices = _resourcesRepository.GetResourceActiveDeviceInfo();
                        var reply = new DeviceInfoList();
                        if (devices?.Count > 0)
                        {
                            foreach (var item in devices)
                            {
                                reply.Devices.Add(item.GetProtoDeviceInfo());
                            }
                        }
                        await responseStreamsActiveDevices[sid].WriteAsync(reply);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "OnSendActiveDevices write to response Stream");
                    }
                });
            }
        }


        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override async Task Devices(Empty request, IServerStreamWriter<DeviceInfoList> responseStream, ServerCallContext context)
        {
            string sid = null;
            try
            {
                var user = context.GetHttpContext().User;
                sid = user.Claims.GetSid();
                if (responseStreamsDevices.ContainsKey(sid))
                    responseStreamsDevices[sid] = responseStream;
                else
                    responseStreamsDevices.Add(sid, responseStream);

                OnSendDevices(sid);
                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);

                await Task.FromResult(true);
            }
            catch (System.OperationCanceledException)
            {
                _logger.LogTrace("Devices connection was canceled from Context Cancellation Token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Devices");
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    if (responseStreamsDevices.ContainsKey(sid))
                        responseStreamsDevices.Remove(sid);
                }
            }
        }

        private void OnSendDevices(string sid)
        {
            if (responseStreamsDevices.ContainsKey(sid))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var devices = _resourcesRepository.GetResourceDeviceInfo();
                        var reply = new DeviceInfoList();
                        if (devices?.Count > 0)
                        {
                            foreach (var item in devices)
                            {
                                reply.Devices.Add(item.GetProtoDeviceInfo());
                            }
                        }
                        await responseStreamsDevices[sid].WriteAsync(reply);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "OnSendDevices write to response Stream");
                    }
                });
            }
        }


        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<DeviceChangeRequest> DeleteDevice(DeviceChangeRequest request, ServerCallContext context)
        {
            return base.DeleteDevice(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override async Task Features(Empty request, IServerStreamWriter<FeatureList> responseStream, ServerCallContext context)
        {
            string sid = null;
            try
            {
                var user = context.GetHttpContext().User;
                sid = user.Claims.GetSid();
                if (responseStreamsFeatures.ContainsKey(sid))
                    responseStreamsFeatures[sid] = responseStream;
                else
                    responseStreamsFeatures.Add(sid, responseStream);

                OnSendFeatures(sid);
                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);

                await Task.FromResult(true);
            }
            catch (System.OperationCanceledException)
            {
                _logger.LogTrace("Features connection was canceled from Context Cancellation Token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Features");
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    if (responseStreamsFeatures.ContainsKey(sid))
                        responseStreamsFeatures.Remove(sid);
                }
            }
        }

        private void OnSendFeatures(string sid)
        {
            if (responseStreamsFeatures.ContainsKey(sid))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var features = _resourcesRepository.GetResourceDeviceFeature();
                        var reply = new FeatureList();
                        if (features?.Count > 0)
                        {
                            foreach (var item in features)
                            {
                                reply.Features.Add(item.GetProtoFeatureDefinition());
                            }
                        }
                        await responseStreamsFeatures[sid].WriteAsync(reply);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "OnSendFeatures write to response Stream");
                    }
                });
            }
        }


        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<FeatureDetails> FeatureDetail(FeatureDetailRequest request, ServerCallContext context)
        {
            return base.FeatureDetail(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<FeatureDetailProfileDeleteRequest> FeatureDetailProfileDelete(FeatureDetailProfileDeleteRequest request, ServerCallContext context)
        {
            return base.FeatureDetailProfileDelete(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<FeatureDetailProfileSaveRequest> FeatureDetailProfileSave(FeatureDetailProfileSaveRequest request, ServerCallContext context)
        {
            return base.FeatureDetailProfileSave(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<FeatureDetailSettingSaveRequest> FeatureDetailSettingSave(FeatureDetailSettingSaveRequest request, ServerCallContext context)
        {
            return base.FeatureDetailSettingSave(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override async Task PendingDevices(Empty request, IServerStreamWriter<DeviceInfoList> responseStream, ServerCallContext context)
        {
            string sid = null;
            try
            {
                var user = context.GetHttpContext().User;
                sid = user.Claims.GetSid();
                if (responseStreamsPendingDevices.ContainsKey(sid))
                    responseStreamsPendingDevices[sid] = responseStream;
                else
                    responseStreamsPendingDevices.Add(sid, responseStream);

                OnSendPendingDevices(sid);
                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);

                await Task.FromResult(true);
            }
            catch (System.OperationCanceledException)
            {
                _logger.LogTrace("PendingDevices connection was canceled from Context Cancellation Token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PendingDevices");
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    if (responseStreamsPendingDevices.ContainsKey(sid))
                        responseStreamsPendingDevices.Remove(sid);
                }
            }
        }

        private void OnSendPendingDevices(string sid)
        {
            if (responseStreamsPendingDevices.ContainsKey(sid))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var features = _resourcesRepository.GetResourcePendingAuthenticationDevices();
                        var reply = new DeviceInfoList();
                        if (features?.Count > 0)
                        {
                            foreach (var item in features)
                            {
                                reply.Devices.Add(item.GetProtoDeviceInfo());
                            }
                        }
                        await responseStreamsPendingDevices[sid].WriteAsync(reply);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "OnSendPendingDevices write to response Stream");
                    }
                });
            }
        }


        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<DeviceChangeRequest> ResolvePendingDevice(DevicePendingRequest request, ServerCallContext context)
        {
            return base.ResolvePendingDevice(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<DeviceInfo> UpdateDevice(DeviceInfo request, ServerCallContext context)
        {
            try
            {
                var user = context.GetHttpContext().User;
                var sid = user.Claims.GetSid();

                _resourcesRepository.UpdateDeviceInfo(deviceInfo);

                var resultFeatureState = _featureManager.FeatureStateChange(new CommunicationBase.Models.ExchangeEntity<CommunicationBase.Models.FeatureState>
                {
                    Sid = sid,
                    Entity = new CommunicationBase.Models.FeatureState
                    {
                        CurrentState = (CommunicationBase.Models.FeatureState.State)(int)request.FeatureState_,
                        FeatureId = request.FeatureId
                    }
                });
                return Task.FromResult(new FeatureState
                {
                    FeatureId = resultFeatureState.Entity.FeatureId,
                    FeatureState_ = (FeatureState.Types.State)(int)resultFeatureState.Entity.CurrentState
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateDevice");
                return Task.FromResult(new FeatureState
                {
                    FeatureId = request.FeatureId,
                    FeatureState_ = FeatureState.Types.State.Error
                });
            }
        }
    }
}
