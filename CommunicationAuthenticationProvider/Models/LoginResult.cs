using SharedBase.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationProvider.Models
{
    public class LoginResult
    {
        public LoginState State { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
