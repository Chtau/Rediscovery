using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Grpc.Core;
using Featuredata;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Google.Protobuf;
using SharedBase.Feature;

namespace CommunicationFeatureProvider.ProtoServices
{
    public class FeatureExchangeService : FeatureExchange.FeatureExchangeBase
    {
        private readonly ILogger<FeatureExchangeService> _logger;
        private readonly IFeatureManager _featureManager;
        private static Dictionary<string, IServerStreamWriter<Featuredata.DeviceFeatureData>> responseStreams = new Dictionary<string, IServerStreamWriter<Featuredata.DeviceFeatureData>>();

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
            if (responseStreams.ContainsKey(sid))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await responseStreams[sid].WriteAsync(new Featuredata.DeviceFeatureData
                        {
                            Data = deviceFeatureData.Data,
                            DeviceId = deviceFeatureData.DeviceId,
                            FeatureId = deviceFeatureData.FeatureId.ToString(),
                            ProfileId = deviceFeatureData.ProfileId.EmptyIfNull()
                        });
                    } catch (Exception ex)
                    {
                        _logger.LogError(ex, "SendFeatureData write to response Stream");
                    }
                });
            }
        }

        [Authorize(Policy = "Device")]
        public override async Task ExchangeStream(IAsyncStreamReader<DeviceFeatureData> requestStream, IServerStreamWriter<DeviceFeatureData> responseStream, ServerCallContext context)
        {
            string sid = null;
            try
            {
                var user = context.GetHttpContext().User;
                sid = user.Claims.GetSid();
                FeatureActiveDevices.AddDevice(sid);
                if (responseStreams.ContainsKey(sid))
                    responseStreams[sid] = responseStream;
                else
                    responseStreams.Add(sid, responseStream);

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
            } finally
            {
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    FeatureActiveDevices.RemoveDevice(sid);
                    if (responseStreams.ContainsKey(sid))
                        responseStreams.Remove(sid);
                }
            }
        }

        [Authorize(Policy = "Device")]
        public override Task<FeatureState> ChangeFeatureState(FeatureState request, ServerCallContext context)
        {
            try
            {
                var user = context.GetHttpContext().User;
                var sid = user.Claims.GetSid();

                var resultFeatureState = _featureManager.FeatureStateChange(new ExchangeEntity<CommunicationBase.Models.FeatureState>
                {
                    Sid = sid,
                    Entity = new CommunicationBase.Models.FeatureState
                    {
                        CurrentState = (CommunicationBase.Models.FeatureState.State)(int)request.FeatureState_,
                        FeatureId = request.FeatureId.SafeGuid()
                    }
                });
                return Task.FromResult(new FeatureState
                {
                    FeatureId = resultFeatureState.Entity.FeatureId.ToString(),
                    FeatureState_ = (FeatureState.Types.State)(int)resultFeatureState.Entity.CurrentState
                });
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Request for change Feature state");
                return Task.FromResult(new FeatureState
                {
                    FeatureId = request.FeatureId,
                    FeatureState_ = FeatureState.Types.State.Error
                });
            }
        }

        [Authorize(Policy = "Device")]
        public override Task<FeatureClientData> FeatureClient(FeatureRequest request, ServerCallContext context)
        {
            var result = new FeatureClientData
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
                } else
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
