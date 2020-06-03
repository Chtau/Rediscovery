using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Authentication;
using System.Threading.Tasks;

namespace CommunicationAuthenticationProvider.ProtoServices
{
    public class AuthenticationExchangeService : AuthentionExchange.AuthentionExchangeBase
    {
        private IServerStreamWriter<WelcomeDeviceReply> _responseStream;
        private ServerCallContext _context;

        public event EventHandler<SharedCoreModels.WelcomeDeviceMessage> ReceivedWelcomeDeviceMessage;

        public void SendWelcomeDeviceReply(SharedCoreModels.Enums.ConnectionState connectionState, string token)
        {
            if (_responseStream != null)
            {
                Task.Run(async () =>
                {
                    await _responseStream.WriteAsync(new WelcomeDeviceReply
                    {
                        ConnectionState = (WelcomeDeviceReply.Types.State)(int)connectionState,
                        Token = token
                    });
                });
            }
        }

        public override async Task Welcome(WelcomeDeviceMessage request, IServerStreamWriter<WelcomeDeviceReply> responseStream, ServerCallContext context)
        {
            _context = context;
            _responseStream = responseStream;

            ReceivedWelcomeDeviceMessage?.Invoke(this, new SharedCoreModels.WelcomeDeviceMessage
            {
                DeviceIdentifier = request.DeviceIdentifier,
                DeviceName = request.DeviceName,
                DeviceType = request.DeviceType,
                Idiom = request.Idiom,
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                OSVersion = request.OSVersion,
                Platform = request.Platform
            });
        }
    }
}
