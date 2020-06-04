using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CommunicationAuthenticationProvider.Services
{
    public interface ITokenService
    {
        string CreateNewToken(string sid, string name, string role, DateTime? expireDateTime = null);
        string CreateNewToken(Claim[] claims, DateTime? expireDateTime = null);
    }
}
