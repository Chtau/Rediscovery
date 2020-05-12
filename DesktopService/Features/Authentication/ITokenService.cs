using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Authentication
{
    public interface ITokenService
    {
        string CreateNewToken(string sid, string name);
    }
}
