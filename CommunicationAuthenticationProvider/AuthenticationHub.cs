using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SharedBase.Authentication;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationAuthenticationProvider
{
    [AllowAnonymous]
    public class AuthenticationHub : Hub
    {
        private readonly ILogger<AuthenticationHub> _logger;
        private readonly IAuthenticationManager _authenticationManager;

        public AuthenticationHub(ILoggerFactory loggerFactory,
            IAuthenticationManager authenticationManager)
        {
            _logger = loggerFactory.CreateLogger<AuthenticationHub>();
            _authenticationManager = authenticationManager;
        }

        public async Task Welcome(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            try
            {
                var result = await _authenticationManager.RequestLogin(welcomeDeviceMessage);
                if (result.ResultState == LoginState.Denied)
                {
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.Denied, null);
                }
                else if (result.ResultState == LoginState.Failed)
                {
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.Error, null);
                }
                else if (result.ResultState == LoginState.RequiredAuthorizeKey)
                {
                    await _authenticationManager.AddPendingApprovel(welcomeDeviceMessage);
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.WaitForApprovel, null);
                }
                else if (result.ResultState == LoginState.OK)
                {
                    await OnLogin(welcomeDeviceMessage.DeviceName, result.Token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task OnLogin(string device, string token)
        {
            await OnSendHello(SharedCoreModels.Enums.ConnectionState.OK, token);
            _logger.LogInformation("Send Manifest data to the Client");
            await Clients.Caller.SendAsync("Manifest", _authenticationManager.GetManifest());
        }

        private async Task OnSendHello(SharedCoreModels.Enums.ConnectionState connectionState, string token)
        {
            _logger.LogInformation("Send Hello information to the Client");
            await Clients.Caller.SendAsync("Hello", connectionState, token);
        }
    }
}
