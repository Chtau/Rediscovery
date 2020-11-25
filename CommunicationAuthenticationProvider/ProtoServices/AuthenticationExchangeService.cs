using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using System.Threading.Tasks;
using Rediscovery.Communication.Provider.Authentication.Services;
using Microsoft.Extensions.Logging;
using Rediscovery.Shared.Base.Authentication;
using Rediscovery.Shared.Base.Extensions;

namespace Rediscovery.Communication.Provider.Authentication.ProtoServices
{
    public class AuthenticationExchangeService : ProtoAuthentication.AuthentionExchange.AuthentionExchangeBase
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

        public override async Task<ProtoAuthentication.WelcomeDeviceReply> Welcome(ProtoAuthentication.WelcomeDeviceMessage request, ServerCallContext context)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome message received");
            Console.ResetColor();
            var welcomeDeviceReply = new ProtoAuthentication.WelcomeDeviceReply
            {
                ConnectionState = ProtoAuthentication.WelcomeDeviceReply.Types.State.Offline,
                Token = ""
            };
            try
            {
                _logger.LogTrace("Provider Welcome received");
                await OnReceivedWelcomeDeviceMessage(new Rediscovery.Shared.Base.Connection.WelcomeDeviceMessage
                {
                    DeviceIdentifier = request.DeviceIdentifier,
                }, (result) =>
                {
                    welcomeDeviceReply = new ProtoAuthentication.WelcomeDeviceReply
                    {
                        ConnectionState = (ProtoAuthentication.WelcomeDeviceReply.Types.State)(int)result.State,
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
        
        private async Task OnReceivedWelcomeDeviceMessage(Rediscovery.Shared.Base.Connection.WelcomeDeviceMessage e, Action<Rediscovery.Shared.Base.Connection.WelcomeDeviceReply> callback)
        {
            _logger.LogTrace("Provider received Welcome message from consumer");
            await Task.Run(async () =>
            {
                try
                {
                    var result = await _authenticationManager.RequestLogin(e);
                    if (result.State == LoginState.Denied)
                    {
                        callback.Invoke(new Rediscovery.Shared.Base.Connection.WelcomeDeviceReply
                        {
                            State = Rediscovery.Shared.Base.Connection.Enums.ConnectionState.Denied,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.Failed)
                    {
                        callback.Invoke(new Rediscovery.Shared.Base.Connection.WelcomeDeviceReply
                        {
                            State = Rediscovery.Shared.Base.Connection.Enums.ConnectionState.Error,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.RequiredAuthorizeKey)
                    {
                        callback.Invoke(new Rediscovery.Shared.Base.Connection.WelcomeDeviceReply
                        {
                            State = Rediscovery.Shared.Base.Connection.Enums.ConnectionState.WaitForApprovel,
                            Token = null
                        });
                    }
                    else if (result.State == LoginState.OK)
                    {
                        callback.Invoke(new Rediscovery.Shared.Base.Connection.WelcomeDeviceReply
                        {
                            State = Rediscovery.Shared.Base.Connection.Enums.ConnectionState.OK,
                            Token = _tokenService.CreateNewToken(result.DeviceIdentifier, result.Id.ToString(), result.Role)
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    callback.Invoke(new Rediscovery.Shared.Base.Connection.WelcomeDeviceReply
                    {
                        State = Rediscovery.Shared.Base.Connection.Enums.ConnectionState.Error,
                        Token = null
                    });
                }
            });
        }
    }
}
