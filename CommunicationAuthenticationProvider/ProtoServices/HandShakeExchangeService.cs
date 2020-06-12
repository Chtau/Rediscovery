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

        public override Task<GreetingReply> Greeting(GreetingMessage request, ServerCallContext context)
        {
            // TODO: greeting should replace some function of welcome
            // TODO: check if the device is allowed
            // TODO: add to pending authorization if unknown
            // TODO: if the device is allowed to connect send the Certificate PEM
            var reply = new GreetingReply
            {
                PEM = "",
                CanConnect = GreetingReply.Types.State.None
            };
            try
            {
                _logger.LogTrace("Received Greeting request");
                reply.PEM = _authenticationManager.GetCertificatePEM(request.DeviceIdentifier);
                return Task.FromResult(reply);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greeting");
            }
            return Task.FromResult(reply);
        }

        private async Task OnReceivedGreeting(SharedBase.Connection.GreetingDeviceMessage e, Action<SharedBase.Connection.Enums.AllowConnect> callback)
        {
            _logger.LogTrace("Provider received Greeting message from consumer");
            await Task.Run(async () =>
            {
                try
                {
                    var canConnect = await _authenticationManager.AllowedToLogin(e.DeviceIdentifier);
                    if (canConnect == SharedBase.Connection.Enums.AllowConnect.OK)
                    {
                        callback.Invoke(canConnect);
                    } else if (canConnect == SharedBase.Connection.Enums.AllowConnect.UnkownDevice)
                    {
                        if (await _authenticationManager.AddPendingApprovel(e))
                            callback.Invoke(canConnect);
                        else
                            callback.Invoke(SharedBase.Connection.Enums.AllowConnect.Error);
                    } else
                    {
                        callback.Invoke(canConnect);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    callback.Invoke(SharedBase.Connection.Enums.AllowConnect.Error);
                }
            });
        }
    }
}
