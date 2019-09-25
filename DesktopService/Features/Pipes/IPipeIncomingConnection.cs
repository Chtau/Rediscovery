using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Pipes
{
    public interface IPipeIncomingConnection
    {
        Task ShowCode(string code, string device, DateTime validTill);
    }
}
