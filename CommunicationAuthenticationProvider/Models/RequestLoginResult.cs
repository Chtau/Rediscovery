using SharedBase.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationProvider.Models
{
    public class RequestLoginResult
    {
        public string Token { get; set; }
        public LoginState ResultState { get; set; }
    }
}
