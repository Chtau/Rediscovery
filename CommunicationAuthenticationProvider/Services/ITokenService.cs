using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationProvider.Services
{
    public interface ITokenService
    {
        string CreateNewToken(string sid, string name);
    }
}
