using Grpc.Core;
using Handshake;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationAuthenticationProvider.ProtoServices
{
    public class HandShakeExchangeService : Handshake.HandShakeExchange.HandShakeExchangeBase
    {
        private readonly ILogger<HandShakeExchangeService> _logger;
        private readonly IAuthenticationManager _authenticationManager;

        public HandShakeExchangeService(ILoggerFactory loggerFactory, IAuthenticationManager authenticationManager)
        {
            _logger = loggerFactory.CreateLogger<HandShakeExchangeService>();
            _authenticationManager = authenticationManager;
        }

        public override async Task<GreetingReply> Greeting(GreetingMessage request, ServerCallContext context)
        {
            var reply = new GreetingReply
            {
                PEM = "",
                CanConnect = GreetingReply.Types.State.None
            };
            try
            {
                _logger.LogTrace("Received Greeting request");
                var allowed = await OnReceivedGreeting(new SharedBase.Connection.GreetingDeviceMessage
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
                if (allowed == SharedBase.Connection.Enums.AllowConnect.OK)
                {
                    reply.PEM = _authenticationManager.GetCertificatePEM(request.DeviceIdentifier);
                }
                switch (allowed)
                {
                    case SharedBase.Connection.Enums.AllowConnect.None:
                        reply.CanConnect = GreetingReply.Types.State.None;
                        break;
                    case SharedBase.Connection.Enums.AllowConnect.OK:
                        reply.CanConnect = GreetingReply.Types.State.Ok;
                        break;
                    case SharedBase.Connection.Enums.AllowConnect.Error:
                        reply.CanConnect = GreetingReply.Types.State.Error;
                        break;
                    case SharedBase.Connection.Enums.AllowConnect.Denied:
                        reply.CanConnect = GreetingReply.Types.State.Denied;
                        break;
                    case SharedBase.Connection.Enums.AllowConnect.UnkownDevice:
                        reply.CanConnect = GreetingReply.Types.State.WaitForApprovel;
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

        private async Task<SharedBase.Connection.Enums.AllowConnect> OnReceivedGreeting(SharedBase.Connection.GreetingDeviceMessage e)
        {
            _logger.LogTrace("Provider received Greeting message from consumer");
            return await Task.Run(async () =>
            {
                try
                {
                    var canConnect = await _authenticationManager.AllowedToLogin(e.DeviceIdentifier);
                    if (canConnect == SharedBase.Connection.Enums.AllowConnect.OK)
                    {
                        return canConnect;
                    } else if (canConnect == SharedBase.Connection.Enums.AllowConnect.UnkownDevice)
                    {
                        if (await _authenticationManager.AddPendingApprovel(e))
                            return canConnect;
                        else
                            return SharedBase.Connection.Enums.AllowConnect.Error;
                    } else
                    {
                        return canConnect;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return SharedBase.Connection.Enums.AllowConnect.Error;
                }
            });
        }
    }
}
