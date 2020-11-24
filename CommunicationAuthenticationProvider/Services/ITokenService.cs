using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Rediscovery.Communication.Authentication.Provider.Services
{
    public interface ITokenService
    {
        string CreateNewToken(string sid, string primarySid, string role, DateTime? expireDateTime = null);
        string CreateNewToken(Claim[] claims, DateTime? expireDateTime = null);
    }
}
