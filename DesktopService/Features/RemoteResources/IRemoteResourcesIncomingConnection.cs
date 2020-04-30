using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.RemoteResources
{
    public interface IRemoteResourcesIncomingConnection
    {
        Task ShowCode(string code, string device, DateTime validTill);
    }
}
