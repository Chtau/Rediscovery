using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Featuredata;
using System.Threading.Tasks;

namespace CommunicationFeatureProvider.ProtoServices
{
    public class FeatureExchangeService : FeatureExchange.FeatureExchangeBase
    {
        private IServerStreamWriter<DeviceFeatureData> _responseStream;
        private ServerCallContext _context;

        public event EventHandler<PluginFeature.Models.DeviceFeatureData> ReceivedFeatureData;

        public void SendFeatureData(PluginFeature.Models.DeviceFeatureData deviceFeatureData)
        {
            if (_responseStream != null)
            {
                Task.Run(async () =>
                {
                    await _responseStream.WriteAsync(new DeviceFeatureData
                    {
                        Data = deviceFeatureData.Data,
                        DeviceId = deviceFeatureData.DeviceId,
                        FeatureId = deviceFeatureData.FeatureId.ToString(),
                        ProfileId = deviceFeatureData.ProfileId
                    });
                });
            }
        }

        public override async Task ExchangeStream(IAsyncStreamReader<DeviceFeatureData> requestStream, IServerStreamWriter<DeviceFeatureData> responseStream, ServerCallContext context)
        {
            _context = context;
            _responseStream = responseStream;

            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    ReceivedFeatureData?.Invoke(this, new PluginFeature.Models.DeviceFeatureData(message.DeviceId, message.FeatureId.SafeGuid(), message.ProfileId, message.Data));
                }
            });

            await readTask;
        }
    }
}
