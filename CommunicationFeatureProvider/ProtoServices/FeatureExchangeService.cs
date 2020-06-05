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

namespace CommunicationFeatureProvider.ProtoServices
{
    public class FeatureExchangeService : FeatureExchange.FeatureExchangeBase
    {
        private readonly ILogger<FeatureExchangeService> _logger;
        private Dictionary<string, IServerStreamWriter<DeviceFeatureData>> responseStreams = new Dictionary<string, IServerStreamWriter<DeviceFeatureData>>();

        public event EventHandler<PluginFeature.Models.DeviceFeatureData> ReceivedFeatureData;

        public FeatureExchangeService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FeatureExchangeService>();
        }

        public void SendFeatureData(string sid, PluginFeature.Models.DeviceFeatureData deviceFeatureData)
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
                    await foreach (var message in requestStream.ReadAllAsync())
                    {
                        ReceivedFeatureData?.Invoke(this, new PluginFeature.Models.DeviceFeatureData(message.DeviceId, message.FeatureId.SafeGuid(), message.ProfileId, message.Data));
                    }
                });

                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);

                await readTask;
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
    }
}
