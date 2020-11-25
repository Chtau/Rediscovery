using Rediscovery.Shared.Base.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Provider.Authentication.Models
{
    public class LoginResult
    {
        public LoginState State { get; set; }
        public string Id { get; set; }
        public string DeviceIdentifier { get; set; }
        public string Role { get; set; }
    }
}
