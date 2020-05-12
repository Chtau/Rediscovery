using CommunicationResourceProvider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DesktopService.Features.Authentication
{
    public class TokenService : ITokenService, IAuthenticateService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly SharedConfigurations.DesktopService.Models.IdentityConfiguration _identitySettings;

        public TokenService(ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.IdentityConfiguration> identitySettings,
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings)
        {
            _logger = loggerFactory.CreateLogger<TokenService>();
            _identitySettings = identitySettings.Value;
            _remoteResourceSettings = remoteResourceSettings.Value;
        }

        public string AuthenticationTokenRemoteResourceConsumer(string consumerKey, string roleName)
        {
            if (string.Equals(_remoteResourceSettings.RediscoveryDesktopHubApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase)
                || string.Equals(_remoteResourceSettings.RediscoveryDesktopInfoHubApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase)
                || string.Equals(_remoteResourceSettings.RediscoveryDiscoveryServiceApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase))
            {
                return OnCreateToken(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, consumerKey),
                    new Claim(ClaimTypes.Name, consumerKey),
                    new Claim(ClaimTypes.Role, roleName)
                });
            }
            return null;
        }

        public string CreateNewToken(string sid, string name)
        {
            return OnCreateToken(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, sid),
                    new Claim(ClaimTypes.Name, name),
                });
        }

        private string OnCreateToken(Claim[] claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_identitySettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(180),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
