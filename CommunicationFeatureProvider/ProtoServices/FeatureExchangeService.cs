using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Rediscovery.Shared.Base.Feature;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rediscovery.Shared.Base.Extensions;
using Rediscovery.Communication.Base;

namespace Rediscovery.Communication.Provider.Feature.ProtoServices
{
    public class FeatureExchangeService : ProtoFeaturedata.FeatureExchange.FeatureExchangeBase
    {
        private readonly ILogger<FeatureExchangeService> _logger;
        private readonly IFeatureManager _featureManager;
        private static readonly Dictionary<string, IServerStreamWriter<ProtoFeaturedata.DeviceFeatureData>> _responseStreams = new Dictionary<string, IServerStreamWriter<ProtoFeaturedata.DeviceFeatureData>>();

        public FeatureExchangeService(ILoggerFactory loggerFactory, IFeatureManager featureManager)
        {
            _logger = loggerFactory.CreateLogger<FeatureExchangeService>();
            _featureManager = featureManager;
            _featureManager.SendData += _featureManager_SendData;
        }

        private void _featureManager_SendData(object sender, ExchangeEntity<FeatureData> e)
        {
            OnSendFeatureData(e.Sid, e.Entity);
        }

        private void OnSendFeatureData(string sid, FeatureData deviceFeatureData)
        {
            if (_responseStreams.ContainsKey(sid))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _responseStreams[sid].WriteAsync(new ProtoFeaturedata.DeviceFeatureData
                        {
                            Data = deviceFeatureData.Data,
                            DeviceId = deviceFeatureData.DeviceId,
                            FeatureId = deviceFeatureData.FeatureId.ToString(),
                            ProfileId = deviceFeatureData.ProfileId.EmptyIfNull()
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "SendFeatureData write to response Stream");
                    }
                });
            }
        }

        [Authorize(Policy = "Device")]
        public override async Task ExchangeStream(IAsyncStreamReader<ProtoFeaturedata.DeviceFeatureData> requestStream, IServerStreamWriter<ProtoFeaturedata.DeviceFeatureData> responseStream, ServerCallContext context)
        {
            string sid = null;
            try
            {
                var user = context.GetHttpContext().User;
                sid = user.Claims.GetSid();
                FeatureActiveDevices.AddDevice(sid);
                _responseStreams[sid] = responseStream;

                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync(cancellationToken: context.CancellationToken))
                    {
                        _featureManager.ReceivedData(new ExchangeEntity<FeatureData>
                        {
                            Entity = new FeatureData(message.DeviceId, message.FeatureId.SafeGuid(), message.ProfileId, message.Data, message.IsClientImplementation, message.NativeResourceType),
                            Sid = sid
                        });
                    }
                });

                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);

                await readTask;
            }
            catch (System.OperationCanceledException)
            {
                _logger.LogTrace("ExchangeStream connection was canceled from Context Cancellation Token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExchangeStream");
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    FeatureActiveDevices.RemoveDevice(sid);
                    if (_responseStreams.ContainsKey(sid))
                        _responseStreams.Remove(sid);
                }
            }
        }

        [Authorize(Policy = "Device")]
        public override Task<ProtoFeaturedata.FeatureState> ChangeFeatureState(ProtoFeaturedata.FeatureState request, ServerCallContext context)
        {
            try
            {
                var user = context.GetHttpContext().User;
                var sid = user.Claims.GetSid();

                var resultFeatureState = _featureManager.FeatureStateChange(new ExchangeEntity<Communication.Base.Models.FeatureState>
                {
                    Sid = sid,
                    Entity = new Communication.Base.Models.FeatureState
                    {
                        CurrentState = (Communication.Base.Models.FeatureState.State)(int)request.FeatureState_,
                        FeatureId = request.FeatureId.SafeGuid()
                    }
                });
                return Task.FromResult(new ProtoFeaturedata.FeatureState
                {
                    FeatureId = resultFeatureState.Entity.FeatureId.ToString(),
                    FeatureState_ = (ProtoFeaturedata.FeatureState.Types.State)(int)resultFeatureState.Entity.CurrentState
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request for change Feature state");
                return Task.FromResult(new ProtoFeaturedata.FeatureState
                {
                    FeatureId = request.FeatureId,
                    FeatureState_ = ProtoFeaturedata.FeatureState.Types.State.Error
                });
            }
        }

        [Authorize(Policy = "Device")]
        public override Task<ProtoFeaturedata.FeatureClientData> FeatureClient(ProtoFeaturedata.FeatureRequest request, ServerCallContext context)
        {
            var result = new ProtoFeaturedata.FeatureClientData
            {
                FeatureId = request.FeatureId
            };
            try
            {
                var user = context.GetHttpContext().User;
                var sid = user.Claims.GetSid();

                var featureId = request.FeatureId.SafeGuid();
                var archive = _featureManager.GetFeatureUIArchive(featureId);
                var profiles = _featureManager.GetFeatureProfiles(featureId);
                var setting = _featureManager.GetFeatureSettings(featureId);

                if (setting != null)
                {
                    result.Setting = setting.GetProtoFeatureDetailSetting();
                }
                else
                {
                    result.Setting = new FeatureDetailSetting
                    {
                        Data = "",
                        FeatureId = request.FeatureId
                    };
                }

                if (profiles?.Count > 0)
                {
                    foreach (var item in profiles)
                    {
                        result.Profiles.Add(item.GetProtoFeatureDetailProfile());
                    }
                }
                if (archive?.Length > 0)
                {
                    result.Archive = ByteString.CopyFrom(archive);
                }

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request to get feature client data");
                return Task.FromResult(result);
            }
        }
    }
}