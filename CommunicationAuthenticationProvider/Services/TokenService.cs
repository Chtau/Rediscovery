using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        public string CreateNewToken(string sid, string name, string role, DateTime? expireDateTime = null)
        {
            return OnCreateToken(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, sid),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Role, role),
                }, expireDateTime ?? DateTime.UtcNow.AddDays(7));
        }

        public string CreateNewToken(Claim[] claims, DateTime? expireDateTime = null)
        {
            if (claims?.Count() > 0)
                return OnCreateToken(claims, expireDateTime ?? DateTime.UtcNow.AddDays(7));
            else
                throw new ArgumentNullException("claims", "Jwt Token requires at least one Claim");
        }

        private string OnCreateToken(Claim[] claims, DateTime expireDateTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expireDateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(CommunicationAuthenticationProviderExtensions.TokenSigningKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
