using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.Authentication
{
    public interface IRoleResolver
    {
        string GetRole(string deviceIdentifier);
    }
}
