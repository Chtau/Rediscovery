using CommunicationAuthenticationProvider.Services;
using Microsoft.Extensions.Logging;
using SharedBase.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationAuthenticationProvider
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IEventService _eventService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ITokenService _tokenService;

        public AuthenticationService(ILoggerFactory loggerFactory, IEventService eventService,
            IAuthenticationManager authenticationManager,
            ITokenService tokenService)
        {
            _logger = loggerFactory.CreateLogger<AuthenticationService>();
            _authenticationManager = authenticationManager;
            _eventService = eventService;
            _tokenService = tokenService;
            _eventService.ReceivedWelcomeDeviceMessage += _eventService_ReceivedWelcomeDeviceMessage;
        }

        private void _eventService_ReceivedWelcomeDeviceMessage(object sender, SharedCoreModels.WelcomeDeviceMessage e)
        {
            _logger.LogTrace("Provider received Welcome message from consumer");
            Task.Run(async () =>
            {
                try
                {
                    var result = await _authenticationManager.RequestLogin(e);
                    if (result.State == LoginState.Denied)
                    {
                        _eventService.InvokeSendWelcomeDeviceReply(new SharedCoreModels.WelcomeDeviceReply
                        {
                            State = SharedCoreModels.Enums.ConnectionState.Denied,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.Failed)
                    {
                        _eventService.InvokeSendWelcomeDeviceReply(new SharedCoreModels.WelcomeDeviceReply
                        {
                            State = SharedCoreModels.Enums.ConnectionState.Error,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.RequiredAuthorizeKey)
                    {
                        await _authenticationManager.AddPendingApprovel(e);
                        _eventService.InvokeSendWelcomeDeviceReply(new SharedCoreModels.WelcomeDeviceReply
                        {
                            State = SharedCoreModels.Enums.ConnectionState.WaitForApprovel,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.OK)
                    {
                        _eventService.InvokeSendWelcomeDeviceReply(new SharedCoreModels.WelcomeDeviceReply
                        {
                            State = SharedCoreModels.Enums.ConnectionState.OK,
                            Token = _tokenService.CreateNewToken(result.Id, result.Name)
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            });
        }
    }
}
