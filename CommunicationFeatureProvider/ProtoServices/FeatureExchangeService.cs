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

namespace CommunicationFeatureProvider.ProtoServices
{
    public class FeatureExchangeService : FeatureExchange.FeatureExchangeBase
    {
        private readonly ILogger<FeatureExchangeService> _logger;
        private readonly IFeatureManager _featureManager;
        private Dictionary<string, IServerStreamWriter<DeviceFeatureData>> responseStreams = new Dictionary<string, IServerStreamWriter<DeviceFeatureData>>();

        public FeatureExchangeService(ILoggerFactory loggerFactory, IFeatureManager featureManager)
        {
            _logger = loggerFactory.CreateLogger<FeatureExchangeService>();
            _featureManager = featureManager;
            _featureManager.SendData += _featureManager_SendData;
        }

        private void _featureManager_SendData(object sender, CommunicationBase.Models.ExchangeEntity<PluginFeature.Models.DeviceFeatureData> e)
        {
            OnSendFeatureData(e.Sid, e.Entity);
        }

        private void OnSendFeatureData(string sid, PluginFeature.Models.DeviceFeatureData deviceFeatureData)
        {
            if (responseStreams.ContainsKey(sid))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await responseStreams[sid].WriteAsync(new DeviceFeatureData
                        {
                            Data = deviceFeatureData.Data,
                            DeviceId = deviceFeatureData.DeviceId,
                            FeatureId = deviceFeatureData.FeatureId.ToString(),
                            ProfileId = deviceFeatureData.ProfileId
                        });
                    } catch (Exception ex)
                    {
                        _logger.LogError(ex, "SendFeatureData write to response Stream");
                    }
                });
            }
        }

        [Authorize]
        public override async Task ExchangeStream(IAsyncStreamReader<DeviceFeatureData> requestStream, IServerStreamWriter<DeviceFeatureData> responseStream, ServerCallContext context)
        {
            string sid = null;
            try
            {
                var user = context.GetHttpContext().User;
                sid = user.Claims.GetSid();
                if (responseStreams.ContainsKey(sid))
                    responseStreams[sid] = responseStream;
                else
                    responseStreams.Add(sid, responseStream);

                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync(cancellationToken: context.CancellationToken))
                    {
                        _featureManager.ReceivedData(new CommunicationBase.Models.ExchangeEntity<PluginFeature.Models.DeviceFeatureData>
                        {
                            Entity = new PluginFeature.Models.DeviceFeatureData(message.DeviceId, message.FeatureId.SafeGuid(), message.ProfileId, message.Data),
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
                    if (responseStreams.ContainsKey(sid))
                        responseStreams.Remove(sid);
                }
            }
        }

        [Authorize]
        public override Task<FeatureState> ChangeFeatureState(FeatureState request, ServerCallContext context)
        {
            try
            {
                var user = context.GetHttpContext().User;
                var sid = user.Claims.GetSid();

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

        [Authorize]
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

                result.Setting = setting.GetProtoFeatureDetailSetting();
                if (profiles?.Count > 0)
                {
                    foreach (var item in profiles)
                    {
                        result.Profiles.Add(item.GetProtoFeatureDetailProfile());
                    }
                }
                if (archive?.Length > 0)
                {
                    // TODO: check against max byte array length for each entity in the proto definition
                    result.Archive.Add(ByteString.CopyFrom(archive));
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
