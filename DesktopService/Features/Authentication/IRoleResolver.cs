using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Authentication
{
    public interface IRoleResolver
    {
        string GetRole(string deviceIdentifier);
    }
}
