using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Authentication;
using System.Threading.Tasks;
using CommunicationAuthenticationProvider.Services;
using Microsoft.Extensions.Logging;

namespace CommunicationAuthenticationProvider.ProtoServices
{
    public class AuthenticationExchangeService : AuthentionExchange.AuthentionExchangeBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<AuthenticationExchangeService> _logger;
        private IServerStreamWriter<WelcomeDeviceReply> _responseStream;

        public AuthenticationExchangeService(ILoggerFactory loggerFactory, IEventService eventService)
        {
            _logger = loggerFactory.CreateLogger<AuthenticationExchangeService>();
            _eventService = eventService;
            _eventService.SendWelcomeDeviceReply += _eventService_SendWelcomeDeviceReply;
        }

        private void _eventService_SendWelcomeDeviceReply(object sender, SharedCoreModels.WelcomeDeviceReply e)
        {
            OnSendWelcomeDeviceReply(e);
        }

        private void OnSendWelcomeDeviceReply(SharedCoreModels.WelcomeDeviceReply welcomeDeviceReply)
        {
            _logger.LogTrace("Provider try to send Welcome reply");
            if (_responseStream != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _responseStream.WriteAsync(new WelcomeDeviceReply
                        {
                            ConnectionState = (WelcomeDeviceReply.Types.State)(int)welcomeDeviceReply.State,
                            Token = welcomeDeviceReply.Token
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "OnSendWelcomeDeviceReply");
                    }
                });
            }
        }

        public override async Task Welcome(WelcomeDeviceMessage request, IServerStreamWriter<WelcomeDeviceReply> responseStream, ServerCallContext context)
        {
            try
            {
                _logger.LogTrace("Provider Welcome received");
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

                do
                {
                    await Task.Delay(100);
                } while (true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Welcome");
            }
        }
    }
}
