using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Authentication;
using System.Threading.Tasks;
using CommunicationAuthenticationProvider.Services;

namespace CommunicationAuthenticationProvider.ProtoServices
{
    public class AuthenticationExchangeService : AuthentionExchange.AuthentionExchangeBase
    {
        private readonly IEventService _eventService;
        private IServerStreamWriter<WelcomeDeviceReply> _responseStream;
        private ServerCallContext _context;

        public AuthenticationExchangeService(IEventService eventService)
        {
            _eventService = eventService;
            _eventService.SendWelcomeDeviceReply += _eventService_SendWelcomeDeviceReply;
        }

        private void _eventService_SendWelcomeDeviceReply(object sender, SharedCoreModels.WelcomeDeviceReply e)
        {
            OnSendWelcomeDeviceReply(e);
        }

        private void OnSendWelcomeDeviceReply(SharedCoreModels.WelcomeDeviceReply welcomeDeviceReply)
        {
            if (_responseStream != null)
            {
                Task.Run(async () =>
                {
                    await _responseStream.WriteAsync(new WelcomeDeviceReply
                    {
                        ConnectionState = (WelcomeDeviceReply.Types.State)(int)welcomeDeviceReply.State,
                        Token = welcomeDeviceReply.Token
                    });
                });
            }
        }

        public override async Task Welcome(WelcomeDeviceMessage request, IServerStreamWriter<WelcomeDeviceReply> responseStream, ServerCallContext context)
        {
            _context = context;
            _responseStream = responseStream;

            _eventService.InvokeReceivedWelcomeDeviceMessage(new SharedCoreModels.WelcomeDeviceMessage
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
