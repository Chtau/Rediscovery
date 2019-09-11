using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    public class ConnectHub : Hub
    {
        public async Task Welcome(string user, string identifyer)
        {
            await Clients.Caller.SendAsync("Hello", true);
        }
    }
}
