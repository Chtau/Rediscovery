using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DesktopService.Features.Identity
{
    public class ClaimUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Sid)?.Value;
        }
    }
}
