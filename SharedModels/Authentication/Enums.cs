using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Authentication
{
    public enum LoginState
    {
        Failed,
        Denied,
        RequiredAuthorizeKey,
        OK
    }
}
