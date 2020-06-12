using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Authentication;
using System.Threading.Tasks;
using CommunicationAuthenticationProvider.Services;
using Microsoft.Extensions.Logging;
using SharedBase.Authentication;

namespace CommunicationAuthenticationProvider.ProtoServices
{
    public class AuthenticationExchangeService : AuthentionExchange.AuthentionExchangeBase
    {
        private readonly ILogger<AuthenticationExchangeService> _logger;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ITokenService _tokenService;

        public AuthenticationExchangeService(ILoggerFactory loggerFactory,
            IAuthenticationManager authenticationManager,
            ITokenService tokenService)
        {
            _logger = loggerFactory.CreateLogger<AuthenticationExchangeService>();
            _authenticationManager = authenticationManager;
            _tokenService = tokenService;
        }

        public override async Task<WelcomeDeviceReply> Welcome(WelcomeDeviceMessage request, ServerCallContext context)
        {
            var welcomeDeviceReply = new WelcomeDeviceReply
            {
                ConnectionState = WelcomeDeviceReply.Types.State.Offline,
                Token = ""
            };
            try
            {
                _logger.LogTrace("Provider Welcome received");
                await OnReceivedWelcomeDeviceMessage(new SharedBase.Connection.WelcomeDeviceMessage
                {
                    DeviceIdentifier = request.DeviceIdentifier,
                    DeviceName = request.DeviceName,
                    DeviceType = request.DeviceType,
                    Idiom = request.Idiom,
                    Manufacturer = request.Manufacturer,
                    Model = request.Model,
                    OSVersion = request.OSVersion,
                    Platform = request.Platform
                }, (result) =>
                {
                    welcomeDeviceReply = new WelcomeDeviceReply
                    {
                        ConnectionState = (WelcomeDeviceReply.Types.State)(int)result.State,
                        Token = result.Token.EmptyIfNull()
                    };
                });
                welcomeDeviceReply.Token.EmptyIfNull();
                return welcomeDeviceReply;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Welcome");
                return welcomeDeviceReply;
            }
        }

        private async Task OnReceivedWelcomeDeviceMessage(SharedBase.Connection.WelcomeDeviceMessage e, Action<SharedBase.Connection.WelcomeDeviceReply> callback)
        {
            _logger.LogTrace("Provider received Welcome message from consumer");
            await Task.Run(async () =>
            {
                try
                {
                    var result = await _authenticationManager.RequestLogin(e);
                    if (result.State == LoginState.Denied)
                    {
                        callback.Invoke(new SharedBase.Connection.WelcomeDeviceReply
                        {
                            State = SharedBase.Connection.Enums.ConnectionState.Denied,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.Failed)
                    {
                        callback.Invoke(new SharedBase.Connection.WelcomeDeviceReply
                        {
                            State = SharedBase.Connection.Enums.ConnectionState.Error,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.RequiredAuthorizeKey)
                    {
                        await _authenticationManager.AddPendingApprovel(e);
                        callback.Invoke(new SharedBase.Connection.WelcomeDeviceReply
                        {
                            State = SharedBase.Connection.Enums.ConnectionState.WaitForApprovel,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.OK)
                    {
                        callback.Invoke(new SharedBase.Connection.WelcomeDeviceReply
                        {
                            State = SharedBase.Connection.Enums.ConnectionState.OK,
                            Token = _tokenService.CreateNewToken(result.Id, result.Name, result.Role)
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    callback.Invoke(new SharedBase.Connection.WelcomeDeviceReply
                    {
                        State = SharedBase.Connection.Enums.ConnectionState.Error,
                        Token = null
                    });
                }
            });
        }
    }
}
