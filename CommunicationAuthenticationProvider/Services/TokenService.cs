using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CommunicationAuthenticationProvider.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;

        public TokenService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TokenService>();
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
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(180),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(CommunicationAuthenticationProviderExtensions.TokenSigningKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
