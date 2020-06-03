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

        public AuthenticationService(ILoggerFactory loggerFactory, IEventService eventService,
            IAuthenticationManager authenticationManager)
        {
            _logger = loggerFactory.CreateLogger<AuthenticationService>();
            _authenticationManager = authenticationManager;
            _eventService = eventService;
            _eventService.ReceivedWelcomeDeviceMessage += _eventService_ReceivedWelcomeDeviceMessage;
        }

        private void _eventService_ReceivedWelcomeDeviceMessage(object sender, SharedCoreModels.WelcomeDeviceMessage e)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = await _authenticationManager.RequestLogin(e);
                    if (result.ResultState == LoginState.Denied)
                    {
                        _eventService.InvokeSendWelcomeDeviceReply(new SharedCoreModels.WelcomeDeviceReply
                        {
                            State = SharedCoreModels.Enums.ConnectionState.Denied,
                            Token = null
                        });
                    }
                    else if (result.ResultState == LoginState.Failed)
                    {
                        _eventService.InvokeSendWelcomeDeviceReply(new SharedCoreModels.WelcomeDeviceReply
                        {
                            State = SharedCoreModels.Enums.ConnectionState.Error,
                            Token = null
                        });
                    }
                    else if (result.ResultState == LoginState.RequiredAuthorizeKey)
                    {
                        await _authenticationManager.AddPendingApprovel(e);
                        _eventService.InvokeSendWelcomeDeviceReply(new SharedCoreModels.WelcomeDeviceReply
                        {
                            State = SharedCoreModels.Enums.ConnectionState.WaitForApprovel,
                            Token = null
                        });
                    }
                    else if (result.ResultState == LoginState.OK)
                    {
                        _eventService.InvokeSendWelcomeDeviceReply(new SharedCoreModels.WelcomeDeviceReply
                        {
                            State = SharedCoreModels.Enums.ConnectionState.OK,
                            Token = result.Token
                        });
                        _logger.LogInformation("TODO: Send Manifest data to the Client");
                        //await OnLogin(e.DeviceName, result.Token);
                        await Clients.Caller.SendAsync("Manifest", _authenticationManager.GetManifest());
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
