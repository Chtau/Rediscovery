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
                PEM = ""
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
    }
}
