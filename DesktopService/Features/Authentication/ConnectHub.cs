using DesktopService.Features.FeatureDefinitions;
using DesktopService.Features.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    [AllowAnonymous]
    public class ConnectHub : Hub
    {
        private readonly ILogger<ConnectHub> _logger;
        private readonly Features.Authentication.IAuth _auth;
        private readonly IManifest _manifest;

        public ConnectHub(ILoggerFactory loggerFactory,
            Features.Authentication.IAuth auth,
            IManifest manifest)
        {
            _logger = loggerFactory.CreateLogger<ConnectHub>();
            _auth = auth;
            _manifest = manifest;
        }
        
        public async Task Welcome(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            try
            {
                var result = await _auth.RequestLogin(welcomeDeviceMessage);
                if (result.Item1 == Auth.LoginState.Denied)
                {
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.Denied, null);
                }
                else if (result.Item1 == Auth.LoginState.Failed)
                {
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.Error, null);
                }
                else if (result.Item1 == Auth.LoginState.RequiredAuthorizeKey)
                {
                    await _auth.AddPendingApprovel(welcomeDeviceMessage);
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.WaitForApprovel, null);
                }
                else if (result.Item1 == Auth.LoginState.OK)
                {
                    await OnLogin(welcomeDeviceMessage.DeviceName, result.Item2.Token);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task OnLogin(string device, string token)
        {
            await OnSendHello(SharedCoreModels.Enums.ConnectionState.OK, token);
            _logger.LogInformation("Send Manifest data to the Client");
            await Clients.Caller.SendAsync("Manifest", _manifest.GetManifest());
        }

        private async Task OnSendHello(SharedCoreModels.Enums.ConnectionState connectionState, string token)
        {
            _logger.LogInformation("Send Hello information to the Client");
            await Clients.Caller.SendAsync("Hello", connectionState, token);
        }
    }
}
