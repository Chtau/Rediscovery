using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Authentication.Provider.ProtoServices
{
    public class HandShakeExchangeService : ProtoHandshake.HandShakeExchange.HandShakeExchangeBase
    {
        private readonly ILogger<HandShakeExchangeService> _logger;
        private readonly IAuthenticationManager _authenticationManager;

        public HandShakeExchangeService(ILoggerFactory loggerFactory, IAuthenticationManager authenticationManager)
        {
            _logger = loggerFactory.CreateLogger<HandShakeExchangeService>();
            _authenticationManager = authenticationManager;
        }

        public override async Task<ProtoHandshake.GreetingReply> Greeting(ProtoHandshake.GreetingMessage request, ServerCallContext context)
        {
            var reply = new ProtoHandshake.GreetingReply
            {
                SSLPort = -1,
                PEM = "",
                CanConnect = ProtoHandshake.GreetingReply.Types.State.None,
                SslActive = false,
            };
            try
            {
                _logger.LogTrace("Received Greeting request");
                var allowed = await OnReceivedGreeting(new Rediscovery.Shared.Base.Connection.GreetingDeviceMessage
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
                if (allowed == Rediscovery.Shared.Base.Connection.Enums.AllowConnect.OK)
                {
                    if (_authenticationManager.GetSSLActive())
                    {
                        reply.SSLPort = _authenticationManager.GetSSLPort();
                        reply.PEM = _authenticationManager.GetCertificatePEM(request.DeviceIdentifier);
                    }
                }
                switch (allowed)
                {
                    case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.None:
                        reply.CanConnect = ProtoHandshake.GreetingReply.Types.State.None;
                        break;
                    case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.OK:
                        reply.CanConnect = ProtoHandshake.GreetingReply.Types.State.Ok;
                        break;
                    case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.Error:
                        reply.CanConnect = ProtoHandshake.GreetingReply.Types.State.Error;
                        break;
                    case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.Denied:
                        reply.CanConnect = ProtoHandshake.GreetingReply.Types.State.Denied;
                        break;
                    case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.UnkownDevice:
                        reply.CanConnect = ProtoHandshake.GreetingReply.Types.State.WaitForApprovel;
                        break;
                    default:
                        break;
                }
                return reply;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greeting");
                return reply;
            }
        }

        private async Task<Rediscovery.Shared.Base.Connection.Enums.AllowConnect> OnReceivedGreeting(Rediscovery.Shared.Base.Connection.GreetingDeviceMessage e)
        {
            _logger.LogTrace("Provider received Greeting message from consumer");
            return await Task.Run(async () =>
            {
                try
                {
                    var canConnect = await _authenticationManager.AllowedToLogin(e.DeviceIdentifier, e);
                    if (canConnect == Rediscovery.Shared.Base.Connection.Enums.AllowConnect.OK)
                    {
                        return canConnect;
                    } else if (canConnect == Rediscovery.Shared.Base.Connection.Enums.AllowConnect.UnkownDevice)
                    {
                        if (await _authenticationManager.AddPendingApprovel(e))
                            return canConnect;
                        else
                            return Rediscovery.Shared.Base.Connection.Enums.AllowConnect.Error;
                    } else
                    {
                        return canConnect;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return Rediscovery.Shared.Base.Connection.Enums.AllowConnect.Error;
                }
            });
        }
    }
}
