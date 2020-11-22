using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Authentication
{
    public enum LoginState
    {
        Failed,
        Denied,
        RequiredAuthorizeKey,
        OK
    }
}
